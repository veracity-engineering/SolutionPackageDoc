using DNV.OAuth.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using System;
using System.Threading.Tasks;

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
				ClientId = "34598bb3-b07f-4187-a32b-d64ef8f086bc",
				Scopes = new[] { "https://dnvglb2cprod.onmicrosoft.com/952bbe91-2a89-48a3-95af-fc96bca5c03a/user_impersonation" },
			};
			services.AddOidc(oidcOptions, o =>
			{
				o.Cookie.MaxAge = TimeSpan.FromMinutes(2);
				o.SlidingExpiration = true;

				o.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents();
				o.Events.OnValidatePrincipal = context =>
				{
					if(!context.Request.Cookies.ContainsKey("session-flag"))
					{
						context.RejectPrincipal();
					}

					return Task.CompletedTask;
				};
				o.Events.OnSignedIn = context =>
				{
					context.Response.Cookies.Append("session-flag", "1");
					return Task.CompletedTask;
				};
			});

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
