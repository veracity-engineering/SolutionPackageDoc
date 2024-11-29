using DNV.ApiClients.Veracity.Identity.PlatformApiV4;
using DNV.ApiClients.Veracity.Identity.PlatformApiV4.Interfaces;
using DNV.OAuth.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using V4.Integration.Demo.SolutionPackage.Sample2.Models;

namespace V4.Integration.Demo.SolutionPackage.Sample2.Controllers
{
	[Authorize]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPlatformApiV4Client _platformApiV4Client;

		public HomeController(ILogger<HomeController> logger, IPlatformApiV4Client platformApiV4Client)
        {
            _logger = logger;
            _platformApiV4Client = platformApiV4Client;
        }

        public IActionResult Index()
        {
            return View();
        }

		public async Task<IActionResult> SignIn()
		{
            var data = await _platformApiV4Client.Me.GetMyInfoAsync();

            var tenant = await _platformApiV4Client.Me.GetMyTenantsAsync();
           
            var app = await _platformApiV4Client.Me.GetMyApplicationsAsync();

            ViewData["my_info"] = data;

            ViewData["my_tenant"] = tenant.FirstOrDefault(x => x.TenantId?.ToString().ToLower() == "73ee7be8-d20a-4b54-a3de-18b39b8f1d67");

            ViewData["my_app"] = app.FirstOrDefault(x => x.TenantId.Value.ToString().ToLower() == "73ee7be8-d20a-4b54-a3de-18b39b8f1d67");

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
