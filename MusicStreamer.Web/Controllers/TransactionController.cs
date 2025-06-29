using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static MusicStreamer.Web.Controllers.DashboardController;

namespace MusicStreamer.Web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public TransactionController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["ApiBaseUrl"]);
            _config = config;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create", new TransactionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel model)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userId = JwtHelper.GetUserIdFromToken(token);

            var dto = new
            {
                UserId = int.Parse(userId),
                Amount = model.Amount,
                MerchantName = "MusicStreamer",
                Description = $"Assinatura do plano #{model.PlanId}"
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/transactions", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Erro ao criar transação: {error}";
                return View("Create", model);
            }

            TempData["OK"] = "Transação criada com sucesso! Agora autorize.";
            return RedirectToAction("Authorize", new { id = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("id").GetInt32() });
        }

        [HttpGet]
        public async Task<IActionResult> Authorize(int id)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var userId = JwtHelper.GetUserIdFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var viewModel = new TransactionAuthorizeViewModel
            {
                TransactionId = id
            };

            var response = await _httpClient.GetAsync($"/api/creditcards/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                viewModel.AvailableCards = JsonSerializer.Deserialize<List<CreditCardDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View("Authorize", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Authorize(TransactionAuthorizeViewModel model)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync($"/api/transactions/{model.TransactionId}/authorize", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Transação recusada: {error}";
                return View(model);
            }

            TempData["OK"] = "Transação autorizada com sucesso!";
            return RedirectToAction("History");
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var userId = JwtHelper.GetUserIdFromToken(token);

            var response = await _httpClient.GetAsync($"/api/transactions/user/{userId}");
            var transactions = new List<TransactionListItemViewModel>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                transactions = JsonSerializer.Deserialize<List<TransactionListItemViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            var model = new TransactionHistoryViewModel
            {
                Pending = transactions.Where(t => t.Status == "Pending").OrderByDescending(t => t.CreatedAt).ToList(),
                Others = transactions.Where(t => t.Status != "Pending").OrderByDescending(t => t.CreatedAt).ToList()
            };

            return View("History", model);
        }
    }

}
