# Veracity - My Services API v3 - This Client
The `DNVGL.Veracity.Services.Api.This` package provides a client to resources available under the 'This' view point of API v3.

This view point is appropriate for service owners integrating with Veracity enabling management of their service and sub-service subscriptions.

Resources retrieved from this view point are from the perspective of a service, 
> Only **Client credentials** authentication is supported by this package.

# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](/articles/PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.Veracity.Services.Api.This`
```

# Example

With the nuget package installed, services for each resource may be individually configured, injected and requested inside your solution.

## 1. Configuration
To configure a resource service, introduce configuration in the form of `OAuthHttpClientOptions`:

 `appsettings.json`
 > The `This` view point only supports Client Credential Flow.
```json
{
	"OAuthHttpClients": [
		...
		{
			"Name": "this-subscribers",
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
Register the service or services using extensions methods available from the `DNVGL.Veracity.Services.Api.This.Extensions` namespace.

`startup.cs`
> Packages from `DNVGL.Veracity.Service.Api` are dependent on the [DNVGL.OAuth.Api.HttpClient](/articles/DNVGL.OAuth.Api.HttpClient.md) package, therefore the HttpClientFactory should also be injected.
```cs
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddOAuthHttpClientFactory(Congiuration.GetSection("OAuthHttpClients").Get<IEnumerable<OAuthHttpClientOptions>>());
	...
	services.AddThisSubscribers("this-subscribers")
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
	private readonly IThisSubscribers _thisSubscribers;
	...
	public TestController(IThisSubscribers thisSubscribers)
	{
		...
		_thisSubscribers = thisSubscribers ?? throw new ArgumentNullException(nameof(thisSubscribers));
		...
	}
	...
	public async Task<IActionResult> FetchSubscribers(int page, int pageSize)
	{
		return Json(await _thisSubscribers.List(page, pageSize));
	}
	...
}
```
---
# Resources
- [Administrators](#administrators)
- [Services](#services)
- [Subscribers](#subscribers)
- [Users](#users)

## Administrators
| Registration method | Service interface |
|--|--|
| `AddThisAdministrators(string clientConfigurationName)` | `IThisAdministrators` |

| Name | Description |
|--|--|
| `Get(string userId)` | Retrieves an individual administrator for the authenticated service. |
| `List(int page, int pageSize)` | Retrieves a collection of administrator references for the authenticated service. |

## Services
| Registration method | Service interface |
|--|--|
| `AddThisServices(string clientConfigurationName)` | `IThisServices` |

| Name | Description |
|--|--|
| `AddSubscription(string serviceId, string userId, SubscriptionOptions options)` | Add a subscription to the authenticated service or nested services. |
| `GetAdministrator(string serviceId, string userId)` | Retrieve an individual administrator reference to a administrator of the authenticated service or nested services. |
| `GetSubscriber(serviceId, tring userId)` | Retrieve an individual user reference to a user which has a subscription to a specified service. |
| `List(int page, int pageSize)` | Retrieve a collection of services the authenticated service has access to. |
| `ListAdministrators(string serviceId, int page int pageSize)` | Retrieve a collection of administrator references of administrators for a specified service. |
| `ListSubscribers(string serviceId, int page, int pageSize)` | Retrieve a collection of user references of users subscribed to a specified service. |
| `NotifySubscribers(string serviceId, NotificationOptions options)` | Send a notification to users subscribed to the authenticated service or nested service. |
| `RemoveSubscription(string servieId, string userId)` | Remove a user subscription for a user and the authenticated service or a nested service. |

## Subscribers
| Registration method | Service interface |
|--|--|
| `AddThisSubscribers(string clientConfigurationName)` | `IThisSubscribers` |

| Name | Description |
|--|--|
| `Add(string userId, SubscriptionOptions options)` | Add a subscription to the authenticated service for a specified user. |
| `Get(string userId)` | Retrieve a user reference for a user subscribed to the authenticated service. |
| `List(int page, int pageSize)` | Retrieve a collection of user references to users subscribed to the authenticated service. |
| `Remove(string userId)` | Remove a user subscription to the authenticated service by specified user. |

## Users
| Registration method | Service interface |
|--|--|
| `AddThisUsers(string clientConfigurationName)` | `IThisUsers` |

| Name | Description |
|--|--|
| `Create(CreateUserOptions options)` | Create a new user. |
| `Create(params CreateUserOptions[] options)` | Create a collection of new users. |
| `Resolve(string email)` | Retrieves a collection of user references for users with a specified email value. |