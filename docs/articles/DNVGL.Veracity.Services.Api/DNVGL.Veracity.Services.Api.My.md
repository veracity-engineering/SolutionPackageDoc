# Veracity - My Services API v3 - My Client
The `DNVGL.Veracity.Services.Api.My` package provides a client to resources available under the 'My' view point of API v3.

This view point is appropriate if you intend to use Veracity as an identity provider for your application.

Resources retrieved from this view point are from the perspective of the authenticated user, 
> Only **User credentials** authentication is supported by this package.

# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](/articles/PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.Veracity.Services.Api.My`
```

# Example

With the nuget package installed, services for each resource may be individually configured, injected and requested inside your solution.

## 1. Configuration
To configure a resource service, introduce configuration in the form of `OAuthHttpClientOptions`:

 `appsettings.json`
 > The `My` view point only supports User Credential Flow.
```json
{
	"OAuthHttpClients": [
		...
		{
			"Name": "my-profile",
			"Flow": "UserCredentials",
			"BaseUri": <BaseUri>,
			"SubscriptionKey": <SubscriptionKey>,
			"OAuthClientOptions": {
				"Authority": <Authority>,
				"Scopes": [ <Scope> ]
			}
		}
		...
	]
}
```

## 2. Registration
Register the service or services using extensions methods available from the `DNVGL.Veracity.Services.Api.My.Extensions` namespace.

`startup.cs`
> Packages from `DNVGL.Veracity.Service.Api` are dependent on the [DNVGL.OAuth.Api.HttpClient](/articles/DNVGL.OAuth.Api.HttpClient.md) package, therefore the HttpClientFactory should also be injected.
```cs
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddOAuthHttpClientFactory(Congiuration.GetSection("OAuthHttpClients").Get<IEnumerable<OAuthHttpClientOptions>>());
	...
	services.AddMyProfile("my-profile")
	...
}
```

## 3. Request service
Request the service from the constructor by its interface:

`TestController.cs`
```cs
public class TestController : Controller
{
	...
	private readonly IMyProfile _myPofile;
	...
	public TestController(IMyProfile myProfile)
	{
		...
		_myPofile = myProfile ?? throw new ArgumentNullException(nameof(myProfile));
		...
	}
	...
	public async Task<IActionResult> FetchProfile()
	{
		return Json(await _myPofile.Get());
	}
	...
}
```
---
# Resources
- [Companies](#companies)
- [Messages](#messages)
- [Policies](#policies)
- [Profile](#profile)
- [Services](#services)

## Companies
| Registration method | Service interface |
|--|--|
| `AddMyCompanies(string clientConfigurationName)` | `IMyCompanies` |

| Name | Description |
|--|--|
| `List()` | Retrieves a collection of company references for the authenticated user. |

## Messages
| Registration method | Service interface |
|--|--|
| `AddMyMessages(string clientConfigurationName)` | `IMyMessages` |

| Name | Description |
|--|--|
| `List(bool includeRead)` | Retrieves a collection of messages addressed to the authenticated user. |
| `Get(string messageId)` | Retrieves an individual message addressed to the authenticated user. |
| `GetUnreadCount()` | Retrieves the numeric value indicating how many messages have not been marked as read by the authenticated user. |

## Policies
| Registration method | Service interface |
|--|--|
| `AddMyPolicies(string clientConfigurationName)` | `IMyPolicies` |

| Name | Description |
|--|--|
| `ValidatePolicies(string returnUrl)` | Validates all policies for the authenticated user. |
| `ValidatePolicy(string serviceId, string returnUrl, string skipSubscriptionCheck)` | Validates an individual policy for the authenticated user. | 

## Profile
| Registration method | Service interface |
|--|--|
| `AddMyProfile(string clientConfigurationName)` | `IMyProfile` |

| Name | Description |
|--|--|
| `Get()` | Retrieves the user profile for the authenticated user. |

## Services
| Registration method | Service interface |
|--|--|
| `AddMyServices(string clientConfigurationName)` | `IMyServices` |

| Name | Description |
|--|--|
| `List()` | Retrieves a collection of service references for services the authenticated user is subscribed to. |
