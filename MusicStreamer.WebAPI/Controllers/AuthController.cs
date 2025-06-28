using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.AppServices;
using MusicStreamer.Application.DTOs;
using MusicStreamer.Application.Interfaces.AppServices;

namespace MusicStreamer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public AuthController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
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

            var token = await _authService.RenewToken(user);
            return Ok(token);
        }
    }
}
