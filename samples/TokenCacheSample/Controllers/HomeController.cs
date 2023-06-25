using DNV.OAuth.Abstractions;
using DNV.OAuth.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace TokenCacheSample.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private IClientAppBuilder _appBuilder;
        private readonly OAuth2Options _options;

        public HomeController(IClientAppBuilder appBuilder, OAuth2Options options)
		{
			_appBuilder = appBuilder;
            _options = options;
        }

		public async Task<IActionResult> Index()
		{
			try
			{
				var clientApp = _appBuilder.Build(_options);
				this.ViewBag.Account = this.HttpContext.User;
				var result = await clientApp.AcquireTokenSilent(this.HttpContext.User);
				this.ViewBag.Token = result.AccessToken ?? result.IdToken;
			}
			catch(Exception e)
			{
			}

			return View();
		}

		public async Task<IActionResult> Signout()
		{
			await this.HttpContext.SignOutAsync();
			return this.RedirectToAction("Index");
		}
	}
}
