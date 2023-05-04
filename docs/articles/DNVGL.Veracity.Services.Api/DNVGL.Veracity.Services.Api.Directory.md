# Veracity - My Services API v3 - Directory Client
The `DNVGL.Veracity.Services.Api.Directory` package provides a client to resources available under the 'Directory' view point of API v3.

This view point is appropriate for core Veracity applications where resources are not restricted to any context.
> Only **Client credentials** authentication is supported by this package.

# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](/articles/PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.Veracity.Services.Api.Directory`
```

# Example

With the nuget package installed, services for each resource may be individually configured, injected and requested inside your solution.

## 1. Configuration
To configure a resource service, introduce configuration in the form of `OAuthHttpClientOptions`:

 `appsettings.json`
 > The `Directory` view point only supports Client Credential Flow.
```json
{
	"OAuthHttpClients": [
		...
		{
			"Name": "company-directory",
			"Flow": "ClientCredentials",
			"BaseUri": <BaseUri>,
			"SubscriptionKey": <SubscriptionKey>,
			"OAuthClientOptions": {
				"Authority": <Authority>,
				"ClientId": <ClientId>,
				"ClientSecret": <ClientSecret>,
				"Resource": <Resource>,
				"Scopes": [ <Scope> ],
			}
		}
		...
	]
}
```

## 2. Registration
Register the service or services using extensions methods available from the `DNVGL.Veracity.Services.Api.Directory.Extensions` namespace.

`startup.cs`
> Packages from `DNVGL.Veracity.Service.Api` are dependent on the [DNVGL.OAuth.Api.HttpClient](/articles/DNVGL.OAuth.Api.HttpClient.md) package, therefore the HttpClientFactory should also be injected.
```cs
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddOAuthHttpClientFactory(Congiuration.GetSection("OAuthHttpClients").Get<IEnumerable<OAuthHttpClientOptions>>());
	...
	services.AddCompanyDirectory("company-directory")
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
	private readonly ICompanyDirectory _companyDirectory;
	...
	public TestController(ICompanyDirectory companyDirectory)
	{
		...
		_companyDirectory = companyDirectory ?? throw new ArgumentNullException(nameof(companyDirectory));
		...
	}
	...
	public async Task<IActionResult> FetchCompany(string companyId)
	{
		return Json(await _companyDirectory.Get(companyId));
	}
	...
}
```
---
# Resources
- [Companies](#companies)
- [Services](#services)
- [Users](#users)

## Companies
| Registration method | Service interface |
|--|--|
| `AddCompanyDirectory(string clientConfigurationName)` | `ICompanyDirectory` |

| Name | Description |
|--|--|
| `Get(string companyId)` | Retrieves an individual company. |
| `ListUsers(string companyId, int page, int pageSize)` | Retrieves a paginated collection of user references of users affiliated with a company. |

## Services
| Registration method | Service interface |
|--|--|
| `AddServiceDirectory(string clientConfigurationName)` | `IServiceDirectory` |

| Name | Description |
|--|--|
| `Get(string serviceId)` | Retrieves an individual service. |
| `ListUsers(string serviceId, int page, int pageSize)` | Retrieves a paginated collection of user references of users subscribed to a service. |

## Users
| Registration method | Service interface |
|--|--|
| `AddUserDirectory(string clientConfigurationName)` | `IUserDirectory` |

| Name | Description |
|--|--|
| `Get(string userId)` | Retrieves an individual user. |
| `ListByUserId(params string[] userIds)` | Retrieves a collection of users where the id is included in the parameters. |
| `ListByEmail(string email)` | Retrieves a collection of user references by a specified email value. |
| `ListCompanies(string userId)` | Retrieves a collection of company references of companies with which a user is affiliated. |
| `ListServices(string userId, int page, int pageSize)` | Retrieves a collection of service references of services to which a user is subscribed. |
| `GetSubscription(string userId, string serviceId)` | Retrieve an individual subscription for a specified user and service. |