using DNV.OAuth.Core;
using DNV.OAuth.Web;
using DNV.OAuth.Web.Extensions.Multitenancy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.OAuth.Multitenancy.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddOAuthCore();
            //services.AddAuthorizationCore(o =>
            //{
            //    o.AddPolicy(nameof(o.DefaultPolicy),
            //        p => p.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme,
            //            OpenIdConnectDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
            //    var p = o.GetPolicy(nameof(o.DefaultPolicy));

            //    if (p != null)
            //        o.DefaultPolicy = p;
            //});

            var oidcOptions = new OidcOptions
            {
                Authority = "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
                ClientId = "34598bb3-b07f-4187-a32b-d64ef8f086bc",
                Scopes = new[] { "https://dnvglb2cprod.onmicrosoft.com/952bbe91-2a89-48a3-95af-fc96bca5c03a/user_impersonation" },
                ResponseType = OpenIdConnectResponseType.IdToken
            };

            services.AddOidc(oidcOptions, cookieOption => {
                // explicitly set the cookie lifetime to 10 minutes which will suppress default behavior of session cookie lifetime
                // which allows tenant to be able to sign out independently of other tenants
                // cookieOption.ApplySlidingLifetime(TimeSpan.FromMinutes(10));
            })
            .AddMultitenantAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseMultitenancy(path => path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase)
                            || path.StartsWithSegments("/signin", StringComparison.OrdinalIgnoreCase)
                            || path.StartsWithSegments("/signout", StringComparison.OrdinalIgnoreCase)
                            || path.StartsWithSegments("/error", StringComparison.OrdinalIgnoreCase));
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "error",
                    pattern: "/error", new { controller = "Home", action = "Error" });
                endpoints.MapControllerRoute(
                    name: "signout",
                    pattern: "{tenantAlias}/signout", new { controller = "Home", action = "SignOut" });
                endpoints.MapControllerRoute(
                    name: "rootSignout",
                    pattern: "/signout", new { controller = "Home", action = "SignOut" });
                endpoints.MapControllerRoute(
                    name: "signin",
                    pattern: "{tenantAlias}/signin", new { controller = "Home", action = "SignIn" });
                endpoints.MapControllerRoute(
                    name: "rootSignin",
                    pattern: "/signin", new { controller = "Home", action = "SignIn" });
                endpoints.MapControllerRoute(
                    name: "index",
                    pattern: "{tenantAlias}/{*any}", new { controller = "Home", action = "Index" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
