using DNV.OAuth.Web;
using DNV.OAuth.Web.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DNV.OAuth.Demo
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
				builder.AddUserSecrets<Startup>();
			}

            builder.AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
		{
			var oidcOptions = this.Configuration.GetSection("OAuth").Get<OidcOptions>();

            services.AddOidc(oidcOptions)
				.AddJwt(this.Configuration.GetSection("JwtOptions").GetChildren());

			services.AddControllersWithViews();

            // add swagger generation and swagger UI with authentication feature
            services.AddSwagger(o => this.Configuration.Bind("SwaggerOptions", o));

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

		public void Configure(IApplicationBuilder app, IHostEnvironment env)
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
            // provide parameters to swagger UI
            app.UseSwaggerWithUI(o => this.Configuration.Bind("SwaggerOptions", o));
		}
	}
}
