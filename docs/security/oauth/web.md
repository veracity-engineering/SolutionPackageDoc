# DNVGL.OAuth.Web

DNVGL.OAuth.Web is a .NETCore library for developers to simplify the work of setting up OpenId Connection authentication (OIDC) such as Veracity or Azure AD B2C for ASP.NET Core web project.

---

## Package Install

Package Manager Console

```
PM> `Install-Package DNVGL.OAuth.Web`
```

---

## Basic Authentication Usage

To simplify your authentication implementation of Veracity for your ASP.NET Core web project, you need to add 3 blocks of codes to `Startup.cs`.

1. Add namespace reference. 

```csharp
using DNVGL.OAuth.Web;
```

2. Add `AddOidc` extension method to `ConfigureServices`.
```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddOidc(o =>
	{
		o.Authority = "<Authority>";
		o.ClientId = "<ClientId>";
		o.ClientSecret = "<ClientSecret>";
		o.Resource = "<Resource>";
		o.Scopes = new[] { "<Scope>" };
	});
	...
}
```

3. Add `UseAuthentication` and `UseAuthorization` extension methods to `Configure`.
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	...
	app.UseAuthentication().UseAuthorization();
	...
}
```

4. Then you could launch your project and access an action in a controller that decorated with `[Authorize]`, a challenge request will be sent to IDP(Microsoft or Veracity) to start the authentication process, and the `HttpContext` will be filled with authentication result. 

![](../images/DNVGL.OAuth.Web/challenge.png)

5. A sample project is ready for you to try out: [SimpleOidc](//SimpleOidc).

---

## Access Token Cache Usage

If you web project act as an API gateway, you will want to cache users' access tokens to prevent unnecessary token requests. The library uses `MSAL (Microsoft Authentication Library)` to manipulate tokens.

1. Authorization code flow needs to be set to acquire access token, and refresh token is required for MSAL to re-acquire token from IDP if the token exceed its expiration.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	var oidcOptions = new OidcOptions
	{
		Authority = "<Authority>",
		ClientId = "<ClientId>",
		ClientSecret = "<ClientSecret>",
		Resource = "<Resource>",
		Scopes = new[] { "<Scope>", "offline_access" },	// offline_access is required to retrieve refresh_token.
		ResponseType = OpenIdConnectResponseType.Code
	};
	...
}
```

2. To cache the tokens, an implementaion of `IDistributedCache` such as `MemoryDistributedCache` needs to be added.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddDistributedMemoryCache();
	...
}
```

You can also add `RedisCache` instead.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddDistributedRedisCache(o =>
	{
		o.InstanceName = "<InstanceName>";
		o.Configuration = "<Configuration>";
	});
	...
}
```

3. Calling `AddDistributedTokenCache` will have `IDistributedCache` attached to MSAL client app behind the scene, and the token acquiring process will be replaced by MSAL client app.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddDistributedTokenCache(oidcOptions);
	...
}
```

4. Don't forget to add `AddOidc` after what you did previously.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddOidc(oidcOptions);
	...
}
```

5. A sample project is ready for you to try out: [TokenCacheDemo](//TokenCacheDemo).
