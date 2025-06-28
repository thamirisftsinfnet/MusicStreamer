using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers
{
    public class MusicController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public MusicController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["ApiBaseUrl"]);
            _config = config;
        }

        public async Task<IActionResult> Details(int id)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.GetAsync($"/api/musics/{id}?userId={JwtHelper.GetUserIdFromToken(token)}");
            if (!res.IsSuccessStatusCode) return NotFound();

            var json = await res.Content.ReadAsStringAsync();
            var music = JsonSerializer.Deserialize<MusicDetailViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View("MusicDetail", music);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int musicId)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _httpClient.PostAsync($"/api/musics/{musicId}/favorite", null);

            return RedirectToAction("Details", new { id = musicId });
        }
    }
}
