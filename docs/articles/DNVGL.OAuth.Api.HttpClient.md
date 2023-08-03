# OAuth HTTP Client Factory
The `DNVGL.OAuth.Api.HttpClient package` is a .NET library which provides a factory for producing authenticated HttpClients for API integration via OAuth.

Developers can use this library to create HttpClient instances which will be pre-authenticated for API requests based on provided configuration.

This package supports two type of credential authentication:
- User credentials - A user may authenticate by providing a username and password via a UI. 
- Client credentials - A service or application may provide a client id and secret to silently authenticate.

---
# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](~/articles/PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.OAuth.Api.HttpClient`
```

# Basic example

## 1. Configuration
Setup API http client configuration in `appsettings.json` file:

```js
  {
    "ApiHttpClientOptions": [
      {
        "Name": "userCredentialsClient",
        "Flow": "user-credentials",
        "BaseUri": "<BaseUri>",
        "SubscriptionKey": "<SubscriptionKey>"
      },
      {
        "Name": "clientCredentialsClient",
        "Flow":"client-credentials",
        "BaseUri": "<BaseUri>",
        "SubscriptionKey": "<SubscriptionKey>"
        "OAuthClientOptions": {
          "Authority": "<Authority>",
          "ClientId": "<ClientId>",
          "ClientSecret": "<ClientSecret>",
          "Resource": "<Resource>",
          "Scopes": [ "<Scope>", "offline_access" ],
          "CallbackPath": "<CallbackPath>"
        }
      }
    ]
  }

```

The package injects an `IHttpClientFactory` which is able to provide multiple HttpClients for different purposes.  The HttpClients may all be configured through a configuration section in which the individual client configurations are listed with a unique `Name` which is used to request HttpClients with the corresponding configurations.

The configuration shown above lists 2 HttpClients.  The first with name `"userCredentialsClient"` is an example of a configuration which would honour the signed in user's credentials for the API for which it makes requests.  The second with name `"clientCredentialsClient"` provides configuration for a client which would be authenticated via the client credential flow with a client id and secret to make requests in an API.  This configuration would allow us to request either type of HttpClient by requesting it from from the HttpClientFactory by providing one of the two names: `"userCredentialsClient"` or `"clientCredentialsClient"` in the method call to the HttpClientFactory.

## 2. Registration
Call the `ServiceCollection` extension method `AddOAuthHttpClientFactory` to register an instance of the `IHttpClientFactory` in to your project in your `Startup.cs` file.

The below code is retrieving the configuration from the `"OAuthHttpClientOptions"` section defined in `apsettings.json` above.

```cs
public void ConfigureService(IServiceCollection services)
{
  ...
  services.AddOAuthHttpClientFactory(Congiuration.GetSection("ApiHttpClientOptions").Get<IEnumerable<OAuthHttpClientOptions>>());
  ...
}
```

If you require a HttpClient applying the user credential flow you should also include the web authentication (`AddOidc`) and token cache handling (`AddDistributedMemoryCache`) from the [DNVGL.OAuth.Web](~/articles/DNVGL.OAuth.Web.md) package.  Include the NuGet package in your project and call the required methods as below:

```cs
public void ConfigureService(IServiceCollection services)
{
  ...
  services.AddDistributedMemoryCache();
  ...
  var oidcOptions = new OidcOptions
  {
    Authority = "<Authority>",
	  ClientId = "<ClientId>",
	  ClientSecret = "<ClientSecret>",
		Resource = "<Resource>",
	  Scopes = new[] { "<Scope>", "offline_access" },
	  ResponseType = OpenIdConnectResponseType.Code
  };
  services.AddOidc(oidcOptions);
  ...
  services.AddOAuthHttpClientFactory(Congiuration.GetSection("ApiHttpClientOptions").Get<IEnumerable<OAuthHttpClientOptions>>());
  ...
}
```

If you only require HttpClients applying the client credential flow the DNVGL.OAuth.Web package is not required.

## 3. Request a client
Resolve `IHttpClientFactory` to create user-credential or client-credential `HttpClient` to access web API. 
```cs
public class TestController
{
  private readonly IHttpClientFactory _httpClientFactory;

  public TestController(IHttpClientFactory httpClientFactory)
  {
    _httpClientFactory = httpClientFactory;
  }

  public User DoSomethingWithSignInUser(string id)
  {
    var client = _httpClientFactory.Create("userCredentialsClient");
    ...
  }

  public Company DoSomethingWithService(string id)
  {
    var client = _httpClientFactory.Create("clientCredentialsClient");
    ...
  }
}
```