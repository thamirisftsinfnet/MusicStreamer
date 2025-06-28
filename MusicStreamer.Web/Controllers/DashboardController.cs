using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public DashboardController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["ApiBaseUrl"]);
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var model = new DashboardViewModel();

            var userId = JwtHelper.GetUserIdFromToken(token);
            var userName = JwtHelper.GetFirstNameFromToken(token);
            var plan = JwtHelper.GetClaim(token, "subscriptionId") ?? "Nenhum";
            var active = JwtHelper.GetClaim(token, "isSubscriptionActive") == "True";

            model.FirstName = userName;
            model.SubscriptionPlan = plan;
            model.IsSubscriptionActive = active;

            var musicRes = await _httpClient.GetAsync($"/api/musics/{userId}/favorites/musics");
            var bandRes = await _httpClient.GetAsync($"/api/bands/{userId}/favorites/bands");

            if (musicRes.IsSuccessStatusCode)
            {
                var musicJson = await musicRes.Content.ReadAsStringAsync();
                model.FavoriteMusics = JsonSerializer.Deserialize<List<MusicDto>>(musicJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            if (bandRes.IsSuccessStatusCode)
            {
                var bandJson = await bandRes.Content.ReadAsStringAsync();
                model.FavoriteBands = JsonSerializer.Deserialize<List<BandDto>>(bandJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(model);
        }
    }

    public static class JwtHelper
    {
        public static string GetUserIdFromToken(string token) => GetClaim(token, "userId") ?? "0";
        public static string GetFirstNameFromToken(string token) => GetClaim(token, "firstName") ?? "Usuário";

        public static string? GetClaim(string token, string claimName)
        {
            var jwt = token.Split('.')[1];
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(jwt)));
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(claimName, out var value))
                return value.GetString();
            return null;
        }

        private static string PadBase64(string base64) =>
            base64.Length % 4 == 0 ? base64 : base64.PadRight(base64.Length + (4 - base64.Length % 4), '=');
    }
}
