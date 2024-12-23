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
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly VeracityOAuthOptions _oauthOptions;
	private readonly IServicesApiV3Client _apiV4Client;

	public HomeController(
		ITokenAcquisition tokenAcquisition,
		VeracityOAuthOptions oauthOptions,
		IServicesApiV3Client apiV4Client
	)
	{
		_tokenAcquisition = tokenAcquisition;
		_oauthOptions = oauthOptions;
		_apiV4Client = apiV4Client;
	}

	public async Task<IActionResult> Index()
	{
		var logger = this.HttpContext.RequestServices.GetRequiredService<ILogger<HomeController>>();
		var token = await _tokenAcquisition.GetAccessTokenForUserAsync([_oauthOptions.DefaultUserScope]);
		logger.LogInformation("User Token: {token}", token);

		token = await _tokenAcquisition.GetAccessTokenForAppAsync(_oauthOptions.DefaultAppScope);
		logger.LogInformation("App Token: {token}", token);

		var info = await _apiV4Client.My.InfoAsync();
		logger.LogInformation("MyInfo: {info}", JsonSerializer.Serialize(info));
		return this.View();
	}

	public IActionResult Signout()
	{
		return this.SignOut();
	}
}