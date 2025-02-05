using DNV.ApiClients.Veracity.Identity.ServicesApiV3;
using DNV.ApiClients.Veracity.Identity.ServicesApiV3.Interfaces;
using DNV.OAuth.ApiClient;
using DNV.OAuth.Web;
using DNV.Web.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
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

		//services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, o =>
		//{
		//	o.UseSecurityTokenValidator = true;
		//	o.SecurityTokenValidator = new MyTokenValidator();
		//});
		var audiences = configuration.GetSection("ApiSchemes")
			.GetChildren()
			.Select(x => x.GetSection("Audience")?.Value)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Cast<string>()
			.ToArray();

		authBuilder.AddVeracityWebApi(configuration, "ApiSchemes:Api1", "Environment", "Api1");
		authBuilder.AddVeracityWebApi(configuration, "ApiSchemes:Api2", "Environment", "Api2");

		services.Configure<JwtBearerOptions>("Api1", o =>
		{
			o.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = context =>
				{
					return Task.CompletedTask;
				}
			};
			//o.TokenValidationParameters.ValidAudiences ??= audiences;
		});

		services.Configure<JwtBearerOptions>("Api2", o =>
		{
			o.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = context =>
				{
					return Task.CompletedTask;
				}
			};
			//o.TokenValidationParameters.ValidAudiences ??= audiences;
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
