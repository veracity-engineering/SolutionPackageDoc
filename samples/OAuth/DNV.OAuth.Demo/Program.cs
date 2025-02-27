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
using System.IdentityModel.Tokens.Jwt;
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

		//services.AddVeracityWebApp(configuration, "OAuth", "Environment")
		//	.EnableTokenAcquisitionToCallDownstreamApi()
		//	.AddDistributedTokenCaches();
		var authBuilder = services.AddAuthentication(Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme);
		authBuilder.AddVeracityWebApp(configuration, "OAuth", "Environment")
			.EnableTokenAcquisitionToCallDownstreamApi()
			.AddDistributedTokenCaches();

		services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, o =>
		{
			//o.TokenHandler = new MyTokenHandler();
		});

		var audiences = configuration.GetSection("ApiSchemes")
			.GetChildren()
			.Select(x => x.GetSection("Audience")?.Value)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Cast<string>()
			.ToArray();

		authBuilder.AddVeracityWebApi(configuration, "ApiSchemes:Api1", "Environment", "Api1");
		authBuilder.AddVeracityWebApi(configuration, "ApiSchemes:Api2", "Environment", "Api2", useLegacyEndpoint: true);

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
				{ "https://login.veracity.com/a68572e3-63ce-4bc1-acdc-b64943502e9d/v2.0/", "Api1" },
				{ "https://login.microsoftonline.com/a68572e3-63ce-4bc1-acdc-b64943502e9d/v2.0", "Api2" }
			};

			var jwt = new JsonWebToken(value["Bearer ".Length..]);
			issuers.TryGetValue(jwt.Issuer, out var issuer);
			return issuer;
		}

		services.Configure<JwtBearerOptions>("Api1", o =>
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

		services.Configure<JwtBearerOptions>("Api2", o =>
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

		services.AddProblemDetails();

		services.AddApiClientForUser<IServicesApiV3Client, ServicesApiV3Client>("ApiV3", o => configuration.Bind("Apis:ApiV3", o));

		services.AddSwagger(configuration, "Swagger", "Environment");

		var app = builder.Build();

		app.UseRouting();
		app.UseAuthentication().UseAuthorization();

		app.MapDefaultControllerRoute();

		app.UseSwaggerWithUI(configuration, "Swagger", "Environment");

		app.Run();
	}
}
