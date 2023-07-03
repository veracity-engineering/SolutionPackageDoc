using AspNetContextSample.Models;
using DNV.Context.AspNet;
using Microsoft.AspNetCore.Mvc;

namespace AspNetContextSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AspNetContextAccessor<Identity> _accessor;

        public HomeController(ILogger<HomeController> logger,
                              AspNetContextAccessor<Identity> accessor)
        {
            _accessor = accessor;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var identity = _accessor.Context?.Payload;
            ViewData["Name"] = identity?.Name;
            ViewData["Country"] = identity?.Country;
            ViewData["City"] = identity?.City;
            return View();
        }
                
    }
}