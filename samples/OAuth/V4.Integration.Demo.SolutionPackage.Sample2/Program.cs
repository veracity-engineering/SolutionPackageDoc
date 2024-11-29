using DNV.ApiClients.Veracity.Identity.PlatformApiV4.Interfaces;
using DNV.ApiClients.Veracity.Identity.PlatformApiV4;
using DNV.OAuth.Abstractions;
using DNV.OAuth.Web;
using DNV.OAuth.Api.HttpClient.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.

//builder.Services.AddOAuthHttpClients(o =>
//{
//    configuration.Bind("OAuthHttpClients", o);
//});

//builder.Services.AddScoped<IPlatformApiV4Client>(sp =>
//{
//    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("VeracityApiv4:ClientCredentials");
//    return ActivatorUtilities.CreateInstance<PlatformApiV4Client>(sp, httpClient, true);
//});

// add token cache support
builder.Services.AddOidc(o => configuration.Bind("OAuth", o));

builder.Services.AddControllersWithViews();

builder.Services.AddOAuthHttpClients(o =>
{
	configuration.Bind("OAuthHttpClients", o);
});

builder.Services.AddScoped<IPlatformApiV4Client>(sp =>
{
	var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("VeracityApiv4:UserCredentials");
	httpClient.BaseAddress = new Uri("https://api-test.veracity.com/test/graph/v4");
	return ActivatorUtilities.CreateInstance<PlatformApiV4Client>(sp, httpClient, true);
});


var app = builder.Build();

app.UseHttpsRedirection().UseRouting();
app.UseAuthentication().UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
