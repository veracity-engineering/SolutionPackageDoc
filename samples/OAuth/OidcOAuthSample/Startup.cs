using DNV.OAuth.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OidcOAuthSample
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) { Configuration = configuration; }

		public void ConfigureServices(IServiceCollection services)
		{
			var oidcOptions = new OidcOptions
			{
				Authority = "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
				ClientId = "6ea267b9-4dc5-47ff-bf10-5512b5fb4f20",
				Scopes = new[] { "https://dnvglb2cprod.onmicrosoft.com/83054ebf-1d7b-43f5-82ad-b2bde84d7b75/user_impersonation" },
				ResponseType = OpenIdConnectResponseType.Code
			};
			services.AddOidc(oidcOptions);

			services.AddControllersWithViews();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				IdentityModelEventSource.ShowPII = true;
			}

			app.UseHttpsRedirection().UseRouting();
			app.UseAuthentication().UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
