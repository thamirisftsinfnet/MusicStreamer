using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.Interfaces.AppServices;

namespace MusicStreamer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/musics")]
    public class MusicController : ControllerBase
    {
        private readonly IMusicService _musicService;

        public MusicController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int userId, [FromQuery] string? term = null)
        {
            try
            {
                var result = await _musicService.SearchMusicAsync(term, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [Authorize]
        [HttpPost("{id}/favorite")]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId").Value);
                await _musicService.ToggleFavoriteAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
        [HttpGet("{userId}/favorites/musics")]
        public async Task<IActionResult> GetFavoriteMusics(int userId)
        {
            try
            {
                var favorites = await _musicService.GetUserFavoritesAsync(userId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
         
        }
    }
}
