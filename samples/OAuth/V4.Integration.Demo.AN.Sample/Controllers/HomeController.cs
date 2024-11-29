using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using V4.Integration.Demo.AN.Sample.Models;
using Veracity.Core.Api.V4;

namespace V4.Integration.Demo.AN.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVeracityGraphClient _veracityClient;

        public HomeController(ILogger<HomeController> logger, IVeracityGraphClient veracityClient)
        {
            _logger = logger;
            _veracityClient = veracityClient;
        }

        //public IActionResult Index()
        //{
        //    var list = _veracityClient.Me.GetMyInfo();
        //    ViewData["my_info"] = list;
        //    return View();
        //}

		[VeracityAuthorization(RelativePathToTenantSelector = "/home/tenantSelector", ErrorPageLocation = "home/error?message")]
		public async Task<IActionResult> Index()
		{
			try
			{
				var tenantId = Request.Query["tenant_id"].First();
				var data = await _veracityClient.Me.GetMyInfo();

				

				var tenant = await _veracityClient.Tenants.GetTenant(tenantId);
				var app = await _veracityClient.ThisApplication.GetApplication(tenantId);
				//await Task.WhenAll(tenant, data, app);

				ViewData["my_info"] = data;

				ViewData["my_tenant"] = tenant;

				ViewData["my_app"] = app;

				return View();
			}
			catch (Exception ex)
			{
				return Redirect($"Home/Error/?message={ex.Message}");
			}
		}

		[Authorize]
		public async Task<IActionResult> TenantSelector()
		{
			try
			{
				var myinfo = await _veracityClient.Me.GetMyInfo();

				await _veracityClient.Me.VerifyUserPolicy(_veracityClient.ApplicationId.Value, "https://localhost:7002");

				var data = await _veracityClient.Me.GetMyTenantsWithApplication(_veracityClient.ApplicationId.Value);
				if (data.Count == 1)
					return RedirectToAction("Index", new { tenant_id = data.First().TenantId });
				return View(data);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
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
