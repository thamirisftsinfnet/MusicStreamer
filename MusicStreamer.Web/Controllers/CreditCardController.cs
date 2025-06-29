using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static MusicStreamer.Web.Controllers.DashboardController;

namespace MusicStreamer.Web.Controllers
{
    public class CreditCardController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CreditCardController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["ApiBaseUrl"]);
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var userId = JwtHelper.GetUserIdFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
 

            var response = await _httpClient.GetAsync($"/api/creditcards/user/{userId}");
            var cards = new List<CreditCard>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                cards = JsonSerializer.Deserialize<List<CreditCard>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(cards);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreditCardViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreditCardViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var userId = JwtHelper.GetUserIdFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var dto = new
            {
                UserId = int.Parse(userId),
                CardHolderName = model.CardHolderName,
                NumberMasked = "**** **** **** " + model.Number[^4..],
                Expiration = model.Expiration,
                Brand = model.Brand,
                Token = "token-simulado" 
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/creditcards", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Erro: {errorMessage}";
                return View(model);
            }

            TempData["OK"] = "Cartão cadastrado com sucesso.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"/api/creditcards/{id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Erro ao remover cartão.";
            else
                TempData["OK"] = "Cartão removido com sucesso.";

            return RedirectToAction("Index");
        }
    }
}
