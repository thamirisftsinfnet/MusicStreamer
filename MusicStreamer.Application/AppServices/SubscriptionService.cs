using MusicStreamer.Application.DTOs;
using MusicStreamer.Application.Interfaces.AppServices;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.AppServices
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionService _transactionService;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            ITransactionService transactionService,
            IUnitOfWork unitOfWork)
        {
            _subscriptionRepository = subscriptionRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _transactionService = transactionService;
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createSubscriptionDto)
        {
            var user = await _userRepository.GetByIdAsync(createSubscriptionDto.UserId);
            if (user == null)
                throw new InvalidOperationException("Usuário não encontrado");

            if (user.IsActive)
                throw new InvalidOperationException("Usuário já possui uma assinatura ativa");

            if (createSubscriptionDto.PlanType != SubscriptionPlanType.Free)
            {
                var hasCards = await _userRepository.HasCreditCardAsync(user.Id);
                if (!hasCards)
                    throw new InvalidOperationException("Você precisa cadastrar um cartão para assinar esse plano.");
            }

            var monthlyFee = GetMonthlyFeeByPlanType(createSubscriptionDto.PlanType);

            var subscription = new Subscription
            {
                UserId = createSubscriptionDto.UserId,
                PlanType = createSubscriptionDto.PlanType,
                StartDate = DateTime.UtcNow,
                IsActive = true,
                MonthlyFee = monthlyFee
            };

            await _subscriptionRepository.AddAsync(subscription);
            await _unitOfWork.CommitAsync();

            if (createSubscriptionDto.PlanType != SubscriptionPlanType.Free)
            {
                var authDto = new AuthorizeTransactionDto
                {
                    UserId = createSubscriptionDto.UserId,
                    Amount = monthlyFee,
                    MerchantName = "MusicStreamer",
                    Description = $"Assinatura do plano {createSubscriptionDto.PlanType}"
                };

                await _transactionService.AddTransactionAsync(authDto);
            }

            return new SubscriptionDto
            {
                Id = subscription.Id,
                PlanType = subscription.PlanType,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                IsActive = subscription.IsActive,
                MonthlyFee = subscription.MonthlyFee
            };
        }


        public async Task<SubscriptionDto> GetUserSubscriptionAsync(int userId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(userId);
            if (subscription == null)
                return null;

            return new SubscriptionDto
            {
                Id = subscription.Id,
                PlanType = subscription.PlanType,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                IsActive = subscription.IsActive,
                MonthlyFee = subscription.MonthlyFee
            };
        }

        public async Task<SubscriptionDto> UpdateSubscriptionPlanAsync(int userId, SubscriptionPlanType newPlanType)
        {
            var subscription = await _subscriptionRepository.GetByUserIdAsync(userId);
            if (subscription == null)
                throw new InvalidOperationException("Usuário não possui assinatura ativa");

            if (newPlanType != SubscriptionPlanType.Free)
            {
                var hasCard = await _userRepository.HasCreditCardAsync(userId);
                if (!hasCard)
                    throw new InvalidOperationException("Você precisa cadastrar um cartão antes de mudar para este plano.");
            }

            subscription.PlanType = newPlanType;
            subscription.MonthlyFee = GetMonthlyFeeByPlanType(newPlanType);

            if (newPlanType != SubscriptionPlanType.Free)
            {
                var authDto = new AuthorizeTransactionDto
                {
                    UserId = subscription.UserId,
                    Amount = subscription.MonthlyFee,
                    MerchantName = "MusicStreamer",
                    Description = $"Alteração de plano para {newPlanType}"
                };

                await _transactionService.AddTransactionAsync(authDto);
            }

            await _unitOfWork.CommitAsync();

            return new SubscriptionDto
            {
                Id = subscription.Id,
                PlanType = subscription.PlanType,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                IsActive = subscription.IsActive,
                MonthlyFee = subscription.MonthlyFee
            };
        }



        public async Task CancelSubscriptionAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var subscription = await _subscriptionRepository.GetByIdAsync(userId);

            if (subscription == null)
                throw new InvalidOperationException("Usuário não possui assinatura ativa");

            subscription.IsActive = false;
            subscription.EndDate = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> IsUserSubscriptionActiveAsync(int userId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(userId);
            return subscription != null && subscription.IsActive;
        }

        private decimal GetMonthlyFeeByPlanType(SubscriptionPlanType planType)
        {
            return planType switch
            {
                SubscriptionPlanType.Free => 0m,
                SubscriptionPlanType.Premium => 19.90m,
                SubscriptionPlanType.Family => 29.90m,
                _ => 0m
            };
        }
    }
}
