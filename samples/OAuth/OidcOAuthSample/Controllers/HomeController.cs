using DNV.OAuth.Web.Extensions.Mfa;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OidcOAuthSample.Controllers
{
	public class HomeController : Controller
	{
		[AllowAnonymous]
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult SignIn()
		{
            var returnUrl = "https://www.bing.com";
            if (HttpContext.SignedInWithMfa())
            {
                return Redirect(returnUrl);
            }
            else
            {
                HttpContext.ChallengeForMfaAsync(returnUrl);
                return NoContent();
            }
		}

		public async Task<IActionResult> SignOut()
		{
			await this.HttpContext.SignOutAsync();
			return this.RedirectToAction("Index");
		}
	}
}
