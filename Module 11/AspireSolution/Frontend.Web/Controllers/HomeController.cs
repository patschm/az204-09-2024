using Frontend.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Frontend.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClient;

        public HomeController(IHttpClientFactory httpClient, ILogger<HomeController> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation($"Action {HttpContext.Request.Path} is called");
            var client = _httpClient.CreateClient("weather");
            var resp = await client.GetAsync("/weather");
            if (resp.IsSuccessStatusCode)
            {
                var data = await resp.Content.ReadAsStringAsync();
                return View("Index", data);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
