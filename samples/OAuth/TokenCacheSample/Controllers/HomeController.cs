using DNV.OAuth.Abstractions;
using DNV.OAuth.Web;
using DNV.Veracity.Services.Api.Models;
using DNV.Veracity.Services.Api.My.Abstractions;
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
		private readonly IMyProfile _myProfile;

		public HomeController(IMyProfile myProfile)
		{
			_myProfile = myProfile ?? throw new ArgumentNullException(nameof(myProfile));
		}

		public Task<Profile> Index()
		{
			return _myProfile.Get();
		}

		public async Task<IActionResult> Signout()
		{
			await this.HttpContext.SignOutAsync();
			return this.RedirectToAction("Index");
		}
	}
}
