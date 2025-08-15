using DNV.ApiClients.Veracity.Identity.ServicesApiV3;
using DNV.ApiClients.Veracity.Identity.ServicesApiV3.Interfaces;
using DNV.OAuth.ApiClient;
using DNV.OAuth.Web;
using DNV.Web.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

internal class Program
{
	private static void Main(string[] args)
	{
		IdentityModelEventSource.ShowPII = true;
		var builder = WebApplication.CreateBuilder(args);
		var services = builder.Services;
		var configuration = builder.Configuration;

		services.AddControllersWithViews();

		var authBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
		authBuilder.AddVeracityWebApp(configuration, "OAuth", "Environment")
			.EnableTokenAcquisitionToCallDownstreamApi()
			.AddDistributedTokenCaches();

		authBuilder.AddVeracityWebApi(configuration, "ApiSchemes:Api1", "Environment", Consts.Api1);
		authBuilder.AddVeracityWebApi(configuration, "ApiSchemes:Api2", "Environment", Consts.Api2, useLegacyEndpoint: true);

		services.AddProblemDetails();

		services.AddApiClientForUser<IServicesApiV3Client, ServicesApiV3Client>("ApiV3", o => configuration.Bind("Apis:ApiV3", o));

		services.AddSwagger(configuration, "Swagger", "Environment");

		ExtraConfigure(builder);

		var app = builder.Build();

		//app.LoadJwtConfiguration()
		//	.GetAwaiter()
		//	.GetResult();

		app.UseRouting();
		app.UseAuthentication()
			.UseAuthorization();

		app.MapDefaultControllerRoute();

		app.UseSwaggerWithUI(configuration, "Swagger", "Environment");

		app.Run();
	}

	static void ExtraConfigure(WebApplicationBuilder builder)
	{
		var services = builder.Services;
		var configuration = builder.Configuration;

		var events = new JwtBearerEvents
		{
			OnAuthenticationFailed = async context =>
			{
			},
			OnTokenValidated = async context =>
			{
			}
		};

		services.Configure<JwtBearerOptions>(Consts.Api1, o =>
		{
			o.Events = events;
		});

		services.Configure<JwtBearerOptions>(Consts.Api2, o =>
		{
			o.Events = events;
		});

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
}
