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
            var result = await _musicService.SearchMusicAsync(term, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/favorite")]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            int userId = int.Parse(User.FindFirst("userId").Value);
            await _musicService.ToggleFavoriteAsync(id, userId);
            return NoContent();
        }
        [HttpGet("{userId}/favorites/musics")]
        public async Task<IActionResult> GetFavoriteMusics(int userId)
        {
            var favorites = await _musicService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }
    }
}
