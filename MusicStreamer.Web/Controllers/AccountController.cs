using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Models;
using System.Text;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory factory, IConfiguration configuration)
        {
            _httpClient = factory.CreateClient();
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Login inválido.";
                return View(model);
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);
            var token = result.GetProperty("token").GetString();

            HttpContext.Session.SetString("JWT", token);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new
            {
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = null };

            var content = new StringContent(
                JsonSerializer.Serialize(dto, options),
                Encoding.UTF8,
                "application/json");
            var response = await _httpClient.PostAsync("/api/users", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Erro: {errorMessage}";
                return View(model);
            }
            TempData["OK"] = "Operação realizada com sucesso";

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWT");
            return RedirectToAction("Login");
        }
    }
}
