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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionRepository _subscriptionRepository;
        public UserService(
            IUserRepository userRepository,
            IAuthService authService,
            INotificationService notificationService,
            IUnitOfWork unitOfWork, ISubscriptionRepository subscriptionRepository)
        {
            _userRepository = userRepository;
            _authService = authService;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<UserDto> CreateUserAsync(UserDto createUserDto)
        {
            try
            {
                if (await _userRepository.EmailExistsAsync(createUserDto.Email))
                    throw new InvalidOperationException("Email já está em uso");

                var user = new User
                {
                    Email = createUserDto.Email,
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    PasswordHash = await _authService.HashPasswordAsync(createUserDto.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _unitOfWork.BeginTransaction();
                await _userRepository.AddAsync(user);

                await _unitOfWork.CommitAsync();

                var userRetorno = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                };
                await _notificationService.SendWelcomeEmailAsync(user);
                var subscription = new Subscription
                {
                    UserId = userRetorno.Id,
                    PlanType = SubscriptionPlanType.Free,
                    StartDate = DateTime.UtcNow,
                    IsActive = true,
                    MonthlyFee = 0
                };
                _subscriptionRepository.AddAsync(subscription);
                await _unitOfWork.CommitAsync();

               
                return userRetorno;
            }
           catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao criar usuário", ex);
            }
        }

        public async Task<string> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !await _authService.ValidatePasswordAsync(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciais inválidas");

            user.Subscription = await _subscriptionRepository.GetByUserIdAsync(user.Id);

            return await _authService.GenerateJwtTokenAsync(user);
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }
    }
}
