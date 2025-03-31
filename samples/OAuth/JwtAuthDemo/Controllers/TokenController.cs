using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JwtAuthDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
	/// <summary>
	/// Request a token from the token endpoint using client credentials.
	/// </summary>
	/// <param name="tokenEndpoint"></param>
	/// <param name="clientId"></param>
	/// <param name="clientSecret"></param>
	/// <param name="scope"></param>
	/// <returns></returns>
	/// <remarks>
	/// Available tokenEndpoint values:
	/// 
	///		https://login.veracity.com/dnvglb2cprod.onmicrosoft.com/b2c_1a_signinwithadfsidp/oauth2/v2.0/token
	///		https://login.microsoftonline.com/a68572e3-63ce-4bc1-acdc-b64943502e9d/oauth2/v2.0/token
	///		https://login.microsoftonline.com/a68572e3-63ce-4bc1-acdc-b64943502e9d/oauth2/token
	///		https://dnvglb2cprod.b2clogin.com/dnvglb2cprod.onmicrosoft.com/b2c_1a_signinwithadfsidp/oauth2/v2.0/token
	/// 
	/// Available scope values:
	/// 
	///		https://dnvglb2cprod.onmicrosoft.com/83054ebf-1d7b-43f5-82ad-b2bde84d7b75/.default
	///		https://dnvglb2cprod.onmicrosoft.com/83054ebf-1d7b-43f5-82ad-b2bde84d7b75
	///		https://dnvglb2cprod.onmicrosoft.com/dfc0f96d-1c85-4334-a600-703a89a32a4c/.default
	///		https://dnvglb2cprod.onmicrosoft.com/dfc0f96d-1c85-4334-a600-703a89a32a4c
	/// 
	/// </remarks>
	[HttpPost]
	public async Task<object> Post(string tokenEndpoint, string clientId, string clientSecret, string scope)
	{
		var client = new HttpClient();
		var content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			{ "grant_type", "client_credentials" },
			{ "client_id", clientId },
			{ "client_secret", clientSecret },
			{ "scope", scope }
		});
		var response = await client.PostAsync(tokenEndpoint, content);
		var json = JsonObject.Parse(await response.Content.ReadAsStringAsync())!;
		var accessToken = json["access_token"]!.GetValue<string>();
		var claims = new JsonWebToken(accessToken).Claims.Select(x => $"{x.Type}: {x.Value}");
		return new
		{
			AccessToken = accessToken,
			Claims = claims
		};
	}
}
