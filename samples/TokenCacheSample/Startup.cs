using DNV.OAuth.Abstractions;
using DNV.OAuth.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TokenCacheSample
{
	public class Startup
	{
		public Startup(IWebHostEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
            var oidcOptions = this.Configuration.GetSection("OAuth").Get<OidcOptions>();

            services.AddScoped<OAuth2Options>(o => oidcOptions);
			// add memory cache
			services.AddDistributedMemoryCache();

			// add redis cache
			//services.AddDistributedRedisCache(o =>
			//{
			//	o.InstanceName = "localhost";
			//	o.Configuration = "localhost";
			//});

			// add token cache support
			services.AddOidc(oidcOptions);

			services.AddControllersWithViews();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
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
