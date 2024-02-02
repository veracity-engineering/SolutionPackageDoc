using DNV.OAuth.Multitenancy.Demo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using DNV.OAuth.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DNV.OAuth.Multitenancy.Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CookieAuthenticationOptions _cookieOption;

        public HomeController(ILogger<HomeController> logger, IOptionsFactory<CookieAuthenticationOptions> optionsFactory)
        {
            _logger = logger;
            _cookieOption = optionsFactory.Create(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public new async Task SignOut()
        {
            await HttpContext.SignOut(new[] {CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme}, "/");
        }

        [Authorize]
        public new async Task<IActionResult> SignIn()
        {
            return View();
        }
    }
}
