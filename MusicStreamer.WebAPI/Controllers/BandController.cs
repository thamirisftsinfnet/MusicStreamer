using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.AppServices;
using MusicStreamer.Application.Interfaces.AppServices;

namespace MusicStreamer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/bands")]
    public class BandController : ControllerBase
    {
        private readonly IBandService _bandService;

        public BandController(IBandService bandService)
        {
            _bandService = bandService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int userId, [FromQuery] string? term = null)
        {
            var result = await _bandService.SearchBandsAsync(term, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/favorite")]
        public async Task<IActionResult> Favorite(int id)
        {
            int userId = int.Parse(User.FindFirst("userId").Value);
            await _bandService.AddToFavoritesAsync(userId, id);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}/favorite")]
        public async Task<IActionResult> Unfavorite(int id)
        {
            int userId = int.Parse(User.FindFirst("userId").Value);
            await _bandService.RemoveFromFavoritesAsync(userId, id);
            return NoContent();
        }
        [HttpGet("{userId}/favorites/bands")]
        public async Task<IActionResult> GetFavoriteBandss(int userId)
        {
            var favorites = await _bandService.GetUserFavoriteBandsAsync(userId);
            return Ok(favorites);
        }
    }
}
