using DNV.ApiClients.Veracity.Identity.ServicesApiV3;
using DNV.ApiClients.Veracity.Identity.ServicesApiV3.Interfaces;
using DNV.OAuth.Common;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace DNV.OAuth.Demo.Controllers;

[Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly VeracityOAuthOptions _oauthOptions;
	private readonly IServicesApiV3Client _apiV3Client;

	public HomeController(
		ILogger<HomeController> logger,
		ITokenAcquisition tokenAcquisition,
		VeracityOAuthOptions oauthOptions,
		IServicesApiV3Client apiV4Client
	)
	{
		_logger = logger;
		_tokenAcquisition = tokenAcquisition;
		_oauthOptions = oauthOptions;
		_apiV3Client = apiV4Client;
	}

	public async Task<IActionResult> Index()
	{
		var token = await _tokenAcquisition.GetAccessTokenForUserAsync([_oauthOptions.DefaultUserScope]);
		_logger.LogInformation("User Token: {token}", token);

		token = await _tokenAcquisition.GetAccessTokenForAppAsync(_oauthOptions.DefaultAppScope);
		_logger.LogInformation("App Token: {token}", token);

		var myServices = await _apiV3Client.My.MyServicesAsync();
		this.ViewBag.Services = myServices;
		_logger.LogInformation("My Services: {services}", JsonSerializer.Serialize(myServices));
		return this.View();
	}

	public IActionResult Signout()
	{
		return this.SignOut();
	}
}