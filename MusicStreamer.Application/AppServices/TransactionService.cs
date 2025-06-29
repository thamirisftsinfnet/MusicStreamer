using Microsoft.Extensions.Configuration;
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
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        private readonly decimal _dailyLimit;
        private readonly decimal _monthlyLimit;
        private readonly decimal _singleTransactionLimit;
        private readonly int _duplicateTransactionWindowMinutes;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;

            _dailyLimit = decimal.Parse(_configuration["Transaction:DailyLimit"] ?? "1000");
            _monthlyLimit = decimal.Parse(_configuration["Transaction:MonthlyLimit"] ?? "5000");
            _singleTransactionLimit = decimal.Parse(_configuration["Transaction:SingleTransactionLimit"] ?? "500");
            _duplicateTransactionWindowMinutes = int.Parse(_configuration["Transaction:DuplicateWindowMinutes"] ?? "2");
        }

        public async Task<TransactionAuthorizationResult> AuthorizeTransactionAsync(int id)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(id);
                if (transaction == null)
                {
                    return new TransactionAuthorizationResult
                    {
                        IsAuthorized = false,
                        DenialReason = "Transação não encontrada"
                    };
                }

                if (transaction.Status == TransactionStatus.Authorized)
                {
                    return new TransactionAuthorizationResult
                    {
                        IsAuthorized = false,
                        DenialReason = "Transação já autorizada anteriormente"
                    };
                }

                var user = await _userRepository.GetByIdAsync(transaction.UserId);
                if (user == null)
                {
                    return new TransactionAuthorizationResult
                    {
                        IsAuthorized = false,
                        DenialReason = "Usuário não encontrado"
                    };
                }

                var subscription = await _subscriptionRepository.GetByUserIdAsync(transaction.UserId);
                if (subscription == null || !subscription.IsActive)
                {
                    return new TransactionAuthorizationResult
                    {
                        IsAuthorized = false,
                        DenialReason = "Assinatura inativa"
                    };
                }

                var dto = new AuthorizeTransactionDto
                {
                    UserId = transaction.UserId,
                    Amount = transaction.Amount,
                    MerchantName = transaction.MerchantName,
                    Description = transaction.Description
                };

                var limitsCheck = await ValidateTransactionLimitsAsync(dto);
                if (!limitsCheck.IsAuthorized)
                {
                    return limitsCheck;
                }

                transaction.Status = TransactionStatus.Authorized;
                transaction.AuthorizationCode = GenerateAuthorizationCode();
                transaction.AuthorizedAt = DateTime.UtcNow;

                _transactionRepository.Update(transaction);
                await _unitOfWork.CommitAsync();

                await _notificationService.SendTransactionNotificationAsync(transaction);

                return new TransactionAuthorizationResult
                {
                    IsAuthorized = true,
                    AuthorizationCode = transaction.AuthorizationCode,
                    Transaction = new TransactionDto
                    {
                        Id = transaction.Id,
                        UserId = transaction.UserId,
                        MerchantName = transaction.MerchantName,
                        Amount = transaction.Amount,
                        Description = transaction.Description,
                        CreatedAt = transaction.CreatedAt,
                        Status = transaction.Status,
                        AuthorizationCode = transaction.AuthorizationCode,
                        AuthorizedAt = transaction.AuthorizedAt
                    }
                };
            }
            catch
            {
                return new TransactionAuthorizationResult
                {
                    IsAuthorized = false,
                    DenialReason = "Erro interno do sistema"
                };
            }
        }

        public async Task<TransactionAuthorizationResult> AddTransactionAsync(AuthorizeTransactionDto authorizeTransactionDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(authorizeTransactionDto.UserId);
                if (user == null)
                {
                    return new TransactionAuthorizationResult
                    {
                        IsAuthorized = false,
                        DenialReason = "Usuário não encontrado"
                    };
                }

                var subscription = await _subscriptionRepository.GetByUserIdAsync(authorizeTransactionDto.UserId);
                if (subscription == null || !subscription.IsActive)
                {
                    return new TransactionAuthorizationResult
                    {
                        IsAuthorized = false,
                        DenialReason = "Assinatura inativa"
                    };
                }

                var authorizationResult = await ValidateTransactionLimitsAsync(authorizeTransactionDto);
                if (!authorizationResult.IsAuthorized)
                {
                    return authorizationResult;
                }

                var transaction = new Transaction
                {
                    UserId = authorizeTransactionDto.UserId,
                    MerchantName = authorizeTransactionDto.MerchantName,
                    Amount = authorizeTransactionDto.Amount,
                    Description = authorizeTransactionDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    AuthorizationCode = GenerateAuthorizationCode(),
                    Status = TransactionStatus.Pending,
                    User = user
                };

                await _transactionRepository.AddAsync(transaction);
                await _unitOfWork.CommitAsync();

                await _notificationService.SendTransactionPendenteNotificationAsync(transaction);

                return new TransactionAuthorizationResult
                {
                    IsAuthorized = true,
                    AuthorizationCode = transaction.AuthorizationCode,
                    Transaction = new TransactionDto
                    {
                        Id = transaction.Id,
                        UserId = transaction.UserId,
                        MerchantName = transaction.MerchantName,
                        Amount = transaction.Amount,
                        Description = transaction.Description,
                        CreatedAt = transaction.CreatedAt,
                        Status = transaction.Status,
                        AuthorizationCode = transaction.AuthorizationCode,
                        AuthorizedAt = transaction.AuthorizedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new TransactionAuthorizationResult
                {
                    IsAuthorized = false,
                    DenialReason = "Erro interno do sistema"
                };
            }
        }
        public async Task<TransactionDto> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new InvalidOperationException("Transação não encontrada");

            return new TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                MerchantName = transaction.MerchantName,
                Amount = transaction.Amount,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt,
                Status = transaction.Status,
                AuthorizationCode = transaction.AuthorizationCode,
                AuthorizedAt = transaction.AuthorizedAt
            };
        }

        public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 20)
        {
            var transactions = await _transactionRepository.GetUserTransactionsAsync(userId, page, pageSize);

            return transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                MerchantName = t.MerchantName,
                Amount = t.Amount,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                Status = t.Status,
                AuthorizationCode = t.AuthorizationCode,
                AuthorizedAt = t.AuthorizedAt
            });
        }

        public async Task<decimal> GetUserSpendingTodayAsync(int userId)
        {
            return await _transactionRepository.GetTotalSpentTodayAsync(userId);
        }

        public async Task<decimal> GetUserSpendingThisMonthAsync(int userId)
        {
            return await _transactionRepository.GetTotalSpentThisMonthAsync(userId);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(userId, startDate, endDate);

            return transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                MerchantName = t.MerchantName,
                Amount = t.Amount,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                Status = t.Status,
                AuthorizationCode = t.AuthorizationCode,
                AuthorizedAt = t.AuthorizedAt
            });
        }

        private async Task<TransactionAuthorizationResult> ValidateTransactionLimitsAsync(AuthorizeTransactionDto dto)
        {
            if (dto.Amount > _singleTransactionLimit)
            {
                return new TransactionAuthorizationResult
                {
                    IsAuthorized = false,
                    DenialReason = $"Valor excede o limite por transação (R$ {_singleTransactionLimit:F2})"
                };
            }

            var todaySpent = await _transactionRepository.GetTotalSpentTodayAsync(dto.UserId);
            if (todaySpent + dto.Amount > _dailyLimit)
            {
                return new TransactionAuthorizationResult
                {
                    IsAuthorized = false,
                    DenialReason = $"Valor excede o limite diário (R$ {_dailyLimit:F2})"
                };
            }

            var monthSpent = await _transactionRepository.GetTotalSpentThisMonthAsync(dto.UserId);
            if (monthSpent + dto.Amount > _monthlyLimit)
            {
                return new TransactionAuthorizationResult
                {
                    IsAuthorized = false,
                    DenialReason = $"Valor excede o limite mensal (R$ {_monthlyLimit:F2})"
                };
            }

            var duplicateWindow = TimeSpan.FromMinutes(_duplicateTransactionWindowMinutes);
            var hasRecentTransaction = await _transactionRepository.HasRecentTransactionAsync(
                dto.UserId, dto.MerchantName, duplicateWindow);

            if (hasRecentTransaction)
            {
                return new TransactionAuthorizationResult
                {
                    IsAuthorized = false,
                    DenialReason = "Transação muito próxima de uma anterior com o mesmo comerciante"
                };
            }

            return new TransactionAuthorizationResult { IsAuthorized = true };
        }

        private string GenerateAuthorizationCode()
        {
            return $"AUTH{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }
    }
}
