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
            if (musicRes.IsSuccessStatusCode)
            {
                var musicJson = await musicRes.Content.ReadAsStringAsync();
                model.FavoriteMusics = JsonSerializer.Deserialize<List<MusicDto>>(musicJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            var bandRes = await _httpClient.GetAsync($"/api/bands/{userId}/favorites/bands");
            if (bandRes.IsSuccessStatusCode)
            {
                var bandJson = await bandRes.Content.ReadAsStringAsync();
                model.FavoriteBands = JsonSerializer.Deserialize<List<BandDto>>(bandJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            var txRes = await _httpClient.GetAsync($"/api/transactions/user/{userId}");
            if (txRes.IsSuccessStatusCode)
            {
                var txJson = await txRes.Content.ReadAsStringAsync();
                var allTx = JsonSerializer.Deserialize<List<TransactionListItemViewModel>>(txJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                model.PendingTransactions = allTx
                    .Where(tx => tx.Status == "Pending")
                    .OrderByDescending(tx => tx.CreatedAt)
                    .Take(3)
                    .ToList();
            }

            return View(model);
        }
       
    }
}
