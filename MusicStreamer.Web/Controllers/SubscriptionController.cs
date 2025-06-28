using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public SubscriptionController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["ApiBaseUrl"]);
            _config = config;
        }

        [HttpGet]
        public IActionResult Manage()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var vm = new SubscriptionViewModel
            {
                CurrentPlan = JwtHelper.GetClaim(token, "subscriptionId") ?? "Nenhum",
                IsActive = JwtHelper.GetClaim(token, "isSubscriptionActive") == "True"
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePlan(SubscriptionViewModel model)
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var userId = JwtHelper.GetUserIdFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var dto = model.ToPlanDto(Convert.ToInt32(userId));
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/subscriptions/{userId}/plan", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Erro ao atualizar o plano.";
                return RedirectToAction("Manage");
            }
            var authResponse = await _httpClient.PostAsync($"/api/auth/renew?userId={userId}", null);

            if (authResponse.IsSuccessStatusCode)
            {
                token = await authResponse.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("JWT", token);
            }

            TempData["OK"] = "Operação realizada com sucesso";

            return RedirectToAction("Manage", model);
        }


        [HttpPost]
        public async Task<IActionResult> Cancel()
        {
            var token = HttpContext.Session.GetString("JWT");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var userId = JwtHelper.GetUserIdFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"/api/subscriptions/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Erro ao cancelar a assinatura.";
            }

            return RedirectToAction("Manage");
        }
    }
}
