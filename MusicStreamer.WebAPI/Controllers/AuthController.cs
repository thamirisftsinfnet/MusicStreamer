using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.AppServices;
using MusicStreamer.Application.DTOs;
using MusicStreamer.Application.Interfaces.AppServices;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ISubscriptionService _subscriptionService;

        public AuthController(IUserService userService, IAuthService authService, ISubscriptionService subscriptionService)
        {
            _userService = userService;
            _authService = authService;
            _subscriptionService = subscriptionService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _userService.AuthenticateAsync(loginDto);
            return Ok(new { Token = token });
        }
        [HttpPost("renew")]
        public async Task<IActionResult> RenewToken([FromQuery] int userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound("Usuário não encontrado.");
            var dto = await _subscriptionService.GetUserSubscriptionAsync(user.Id);
            if (dto != null)
            {
                user.Subscription = new Subscription
                {
                    Id = dto.Id,
                    UserId = userId,
                    PlanType = dto.PlanType,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive,
                    MonthlyFee = dto.MonthlyFee
                };
            }

            var token = await _authService.RenewToken(user);
            return Ok(token);
        }
    }
}
