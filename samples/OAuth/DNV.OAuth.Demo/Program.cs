using DNV.ApiClients.Veracity.Identity.ServicesApiV3;
using DNV.ApiClients.Veracity.Identity.ServicesApiV3.Interfaces;
using DNV.OAuth.ApiClient;
using DNV.OAuth.Common;
using DNV.OAuth.Web;
using DNV.OAuth.Web.Extensions.Mfa;
using DNV.OAuth.Web.Extensions.Multitenancy;
using DNV.OAuth.Web.Extensions.Policy;
using DNV.Web.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

IdentityModelEventSource.ShowPII = true;
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddProblemDetails();
services.AddControllersWithViews();

var authBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
authBuilder.AddVeracityWebApp(
	o =>
	{
		configuration.GetSection("OAuth").Bind(o);
		o.AddMfaSupport(_ => true);
	},
	configuration.GetValue<VeracityEnvironment>("Environment")
)
	.EnableTokenAcquisitionToCallDownstreamApi()
	.AddDistributedTokenCaches();

//services.AddStackExchangeRedisCache(o => o.Configuration = "localhost");

authBuilder.AddVeracityWebApi(configuration, "ApiSchemes:Api1", "Environment", Consts.Api1);
authBuilder.AddVeracityWebApi(configuration, "ApiSchemes:Api2", "Environment", Consts.Api2, useLegacyEndpoint: true);

authBuilder.AddMultitenantAuthentication()
	.AddPolicyValidation(o =>
	{
		o.ServiceId = configuration.GetValue<string>("VeracityServiceId")!;
		o.PolicyValidationMode = PolicyValidationMode.All;
	});

services.AddApiClientForUser<IServicesApiV3Client, ServicesApiV3Client>(
	"ApiV3",
	o => configuration.Bind("Apis:ApiV3", o)
);
//services.AddApiClient<IServicesApiV3Client, ServicesApiV3Client>(
//	"ApiV3",
//	o => configuration.Bind("Apis:ApiV3", o)
//).AddWebApiAuthHandler(ApiClientUserType.User, o => configuration.Bind("Apis:ApiV3", o));

services.AddSwagger(configuration, "Swagger", "Environment");

ExtraConfigure(builder);

var app = builder.Build();

app.Use(async (context, next) =>
{
	try
	{
		await next(context);
	}
	catch(MicrosoftIdentityWebChallengeUserException ex) when (ex.InnerException is MsalUiRequiredException)
	{
		await context.ChallengeAsync();
	}
	catch (MsalUiRequiredException)
	{
		await context.ChallengeAsync();
	}
});

app.UseMultitenancy(
	x => x.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase)
	|| x.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
);

app.UseRouting();
app.UseAuthentication()
	.UseAuthorization();

app.MapDefaultControllerRoute();

app.UseSwaggerWithUI(configuration, "Swagger", "Environment");

app.Run();

static void ExtraConfigure(WebApplicationBuilder builder)
{
	var services = builder.Services;
	var configuration = builder.Configuration;

	return;

	var audiences = configuration.GetSection("ApiSchemes")
		.GetChildren()
		.Select(x => x.GetSection("Audience")?.Value)
		.Where(x => !string.IsNullOrWhiteSpace(x))
		.Cast<string>()
		.ToArray();

	services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, o =>
	{
		//o.TokenHandler = new MyTokenHandler();
	});

	services.Configure<JwtBearerOptions>(Consts.Api1, o =>
	{
		o.TokenValidationParameters.LogValidationExceptions = false;
		o.TokenValidationParameters.ValidAudiences ??= audiences;

		o.ForwardDefaultSelector = GetIssuer;

		o.Events = new JwtBearerEvents
		{
			OnAuthenticationFailed = context =>
			{
				return Task.CompletedTask;
			}
		};
	});

	services.Configure<JwtBearerOptions>(Consts.Api2, o =>
	{
		o.TokenValidationParameters.LogValidationExceptions = false;
		o.TokenValidationParameters.ValidAudiences ??= audiences;

		o.ForwardDefaultSelector = GetIssuer;

		o.Events = new JwtBearerEvents
		{
			OnAuthenticationFailed = context =>
			{
				return Task.CompletedTask;
			}
		};
	});

	static string? GetIssuer(HttpContext context)
	{
		if (!context.Request.Headers.TryGetValue("Authorization", out var values))
		{
			return null;
		}

		var value = values.ToString();

		if (!value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}


		var issuers = new Dictionary<string, string>
			{
				{ "https://login.veracity.com/a68572e3-63ce-4bc1-acdc-b64943502e9d/v2.0/", Consts.Api1 },
				{ "https://login.microsoftonline.com/a68572e3-63ce-4bc1-acdc-b64943502e9d/v2.0", Consts.Api2 }
			};

		var jwt = new JsonWebToken(value["Bearer ".Length..]);
		issuers.TryGetValue(jwt.Issuer, out var issuer);
		return issuer;
	}
}
