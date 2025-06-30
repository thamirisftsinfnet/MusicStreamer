using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public SearchController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["ApiBaseUrl"]);
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string term)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var model = new SearchViewModel { SearchTerm = term };

            var userId = JwtHelper.GetUserIdFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var queryMusic = string.IsNullOrWhiteSpace(term)
                ? $"/api/musics/search?userId={userId}"
                : $"/api/musics/search?term={term}&userId={userId}";

            var musicRes = await _httpClient.GetAsync(queryMusic);

            var queryBand = string.IsNullOrWhiteSpace(term)
                ? $"/api/bands/search?userId={userId}"
                : $"/api/bands/search?term={term}&userId={userId}";

            var bandRes = await _httpClient.GetAsync(queryBand);
            if (musicRes.IsSuccessStatusCode)
            {
                var json = await musicRes.Content.ReadAsStringAsync();
                model.Musics = JsonSerializer.Deserialize<List<MusicResult>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            if (bandRes.IsSuccessStatusCode)
            {
                var json = await bandRes.Content.ReadAsStringAsync();
                model.Bands = JsonSerializer.Deserialize<List<BandResult>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavoriteMusic(int musicId)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _httpClient.PostAsync($"/api/musics/{musicId}/favorite", null);

            return RedirectToAction("Index", new { term = Request.Query["term"] });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavoriteBand(int bandId)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _httpClient.PostAsync($"/api/bands/{bandId}/favorite", null);

            return RedirectToAction("Index", new { term = Request.Query["term"] });
        }
    }
}
