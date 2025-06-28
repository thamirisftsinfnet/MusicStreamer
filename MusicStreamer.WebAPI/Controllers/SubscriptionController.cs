using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs;
using MusicStreamer.Application.Interfaces.AppServices;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionDto dto)
        {
            var result = await _subscriptionService.CreateSubscriptionAsync(dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{userId}/plan")]
        public async Task<IActionResult> UpdatePlan(int userId, [FromBody] CreateSubscriptionDto dto)
        {
            if (userId != dto.UserId)
                return BadRequest("O ID da URL não corresponde ao do corpo da requisição.");

            var updated = await _subscriptionService.UpdateSubscriptionPlanAsync(dto.UserId, dto.PlanType);
            return Ok(updated);
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Cancel(int userId)
        {
            await _subscriptionService.CancelSubscriptionAsync(userId);
            return NoContent();
        }
    }
}
