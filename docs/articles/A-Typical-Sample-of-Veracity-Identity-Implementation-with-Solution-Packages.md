

## appsettings.json

```json
"Oidc": {
    "ResponseType": "code",
    "ClientId": "",
    "ClientSecret": "",
    "Scopes": [
      "https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation"
    ],
    "Authority": "https://logintest.veracity.com/tfp/ed815121-cdfa-4097-b524-e2b23cd36eb6/B2C_1A_SignInWithADFSIdp/v2.0"
},
"APIs": [
    {
		"Name": "VeracityAPIsV3",
		"Flow": 0, //"user-credentials",
		"BaseUri": "https://api-test.veracity.com/Veracity/Services/V3/",
		"SubscriptionKey": "",
		"OAuthClientOptions": {
		"Scopes": [
			"https://dnvglb2ctest.onmicrosoft.com/a4a8e726-c1cc-407c-83a0-4ce37f1ce130/user_impersonation"
		],
		"ClientId": "",
		"ClientSecret": "",
		"Authority": "https://logintest.veracity.com/tfp/ed815121-cdfa-4097-b524-e2b23cd36eb6/B2C_1A_SignInWithADFSIdp/v2.0"
		}
	}, {
		"Name": "VeracityAPIsV3",
		"Flow": 1, //"client-credentials",
		"BaseUri": "https://api-test.veracity.com",
		"SubscriptionKey": "",
		"OAuthClientOptions": {
		"Scopes": [ "https://dnvglb2ctest.onmicrosoft.com/efb3e529-2f80-458b-aedf-7f4c8c794b45/.default" ],
		"ClientId": "",
		"ClientSecret": "",
		"Authority": "https://login.microsoftonline.com/ed815121-cdfa-4097-b524-e2b23cd36eb6/v2.0"
		}
    }
]
```


## Startup.cs

```csharp

services.AddOAuthHttpClientFactory(o => configuration.Bind("APIs", o));

services.AddOAuthCore();

var VeracityMyAPIs = "VeracityAPIsV3";
services.AddMyProfile(VeracityMyAPIs)
            .AddMyCompanies(VeracityMyAPIs)
            .AddMyServices(VeracityMyAPIs)
            .AddMyMessages(VeracityMyAPIs)
            .AddThisUsers(VeracityMyAPIs);

services.AddOidc(o => configuration.Bind("Oidc", o))
	.AddPolicyValidation(o =>
    {
        o.PolicyValidationMode = PolicyValidationMode.PlatformTermsAndCondition;
        o.VeracityPolicyApiConfigName = VeracityMyAPIs;
        o.GetReturnUrl = (ctx, retPath) =>
            ctx.TryGetXForwardedHostUrl(out var hostUrl)
                ? $"{hostUrl}{retPath}"
                : $"{ctx.Request.Scheme}://{ctx.Request.Host}{retPath}";
    });
```

## Controller

```csharp
public class ApiController : ControllerBase
{
    private readonly IMyServices _myServices;
    private readonly IThisUsers _thisUsers;
	
	public ApiController(IMyServices myServices, IThisUsers thisUsers)
	{
		_myServices = myServices;
		_thisUsers = thisUsers;
	}
	
	[HttpGet("me/services")]
	public async Task<IActionResult> GetMyServices()
	{
		return Ok(await _myServices.List());		
	}
	
	[HttpGet("this/resolve")]
	public async Task<IActionResult> FindByEmail(string email)
	{
		var users = await _thisUsers.Resolve(email);
		return Ok(users?.FirstOrDefault());
	}
}

```