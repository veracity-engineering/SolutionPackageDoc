# Integration with Role-based authorization in ASP.NET Core
In this section, you learn how to use Role-based authorization in an ASP.NET core project.

> First of all, It's required to have a basic understanding of [Role-based authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-5.0).

Configuration to enable Role-based authorization.
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddAuthentication().AddCookie(o => o.Events.AddCookieValidateHandler());
            //...
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            //Put UseRouting before UseAuthentication and UseAuthorization
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //...
        }
    }
```

Configuration to enable Role-based authorization if `DNVGL.OAuth.Web` is used in authentication.
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddOidc(o =>
            {
                //....
            }, cookieOption => cookieOption.Events.AddCookieValidateHandler());
            //...
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            //Put UseRouting before UseAuthentication and UseAuthorization
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //...
        }
    }
```

Then `AuthorizeAttribute` can be used to decorate an API to perfrom authorization.
```cs
        [HttpGet]
        [Authorize(Roles = "ReadWeather")]
        public IEnumerable<WeatherForecast> Get()
        {
            //... api logic
        }
```

Alternatively, `PermissionAuthorizeAttribute` is still working.
```cs
        [HttpGet]
        [PermissionAuthorize(WeatherPermission.ReadWeather)]
        public IEnumerable<WeatherForecast> Get()
        {
            //... api logic
        }
```