using DNV.OAuth.Api.HttpClient;
using DNV.OAuth.Api.HttpClient.Extensions;
using DNV.OAuth.Web;
using DNV.OAuth.Web.Extensions.Policy;
using DNV.Veracity.Services.Api.My.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNV.OAuth.Demo;

internal class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		var services = builder.Services;
		var configuration = builder.Configuration;

		services.AddControllersWithViews();

		services.AddOidc(o => configuration.Bind("OAuth", o))
			.AddPolicyValidation(o =>
			{
				o.PolicyValidationMode = PolicyValidationMode.All;
				o.VeracityPolicyApiConfigName = "my-profile";
			})
			;

		var options = configuration.GetSection("OAuthHttpClients:0").Get<OAuthHttpClientOptions>();
		services.AddOAuthHttpClient(options)
			.AddMyProfile("my-profile");

		var app = builder.Build();

		app.UseDeveloperExceptionPage();
		app.UseHttpsRedirection().UseRouting();
		app.UseAuthentication().UseAuthorization();
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
		});

		app.Run();
	}
}
