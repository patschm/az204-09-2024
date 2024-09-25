using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace WebSite.Controllers
{
    public class CalculatorController : Controller
    {
        private IFeatureManager _features;
        private readonly IConfiguration _configuration;

        public CalculatorController(IFeatureManager features, IConfiguration configuration)
        {
            _features = features;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var feat = await _features.IsEnabledAsync("subtract");
            ViewBag.FeatureSubtract = feat.ToString().ToLower();
            return View();
        }
        [FeatureGate("help")]
        public async Task<IActionResult> Help()
        {
            ViewBag.FeatureOn = await _features.IsEnabledAsync("help_content");
            return View();
        }
    }
}
