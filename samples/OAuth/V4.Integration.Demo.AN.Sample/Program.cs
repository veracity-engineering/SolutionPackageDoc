using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stardust.Particles;
using System.Drawing;
using Veracity.Common.Authentication;
using Veracity.Core.Api.V4;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var config = builder.Configuration;
var services = builder.Services;

services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

services.AddVeracity(config)//From the base library
    .AddScoped(s => s.GetService<IHttpContextAccessor>().HttpContext.User)
    .AddSingleton((IServiceProvider s) => { return new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())); })
    ;

services.AddVeracityGraphApi<DotNetLogger>(ConfigurationManagerHelper.GetValueOnKey("veracityGraphBaseUrl"))//add the Veracity Graph API to the IoC container
    .AddAuthentication(sharedOptions =>
    {
        sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddVeracityAuthentication(config)//From the base library
    .AddCookie(options =>
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.Cookie.Name = $"__Host-vid.1";
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;

    });

services.AddMvc(options => options.EnableEndpointRouting = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseRouting();

app.UseAuthentication();

app.UseVeracity();

app.UseRouting().UseAuthorization().UseEndpoints(r =>
{
    r.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
    r.MapRazorPages();
});

app.UseVeracityProfilePicture(botBackgroundColor: Color.DarkGray);

app.Run();
