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
            try
            {
                int userId = int.Parse(User.FindFirst("userId").Value);
                await _bandService.AddToFavoritesAsync(userId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
         
        }

        [Authorize]
        [HttpDelete("{id}/favorite")]
        public async Task<IActionResult> Unfavorite(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId").Value);
                await _bandService.RemoveFromFavoritesAsync(userId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
          
        }
        [HttpGet("{userId}/favorites/bands")]
        public async Task<IActionResult> GetFavoriteBandss(int userId)
        {
            try
            {
                var favorites = await _bandService.GetUserFavoriteBandsAsync(userId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }
}
