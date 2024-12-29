using DNV.ApiClients.Veracity.Identity.ServicesApiV3;
using DNV.ApiClients.Veracity.Identity.ServicesApiV3.Interfaces;
using DNV.OAuth.ApiClient;
using DNV.OAuth.Web;
using DNV.Web.Swagger;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllersWithViews();

//services.AddVeracityWebApp(configuration, "OAuth", "Environment")
//	.EnableTokenAcquisitionToCallDownstreamApi()
//	.AddDistributedTokenCaches();
var authBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
authBuilder.AddVeracityWebApp(configuration, "OAuth", "Environment")
	.EnableTokenAcquisitionToCallDownstreamApi()
	.AddDistributedTokenCaches();

authBuilder.AddVeracityWebApi(configuration, "OAuth", "Environment");
authBuilder.AddVeracityWebApi(configuration, "OAuth2", "Environment", "OAuth2", useLegacyEndpoint: true);

//services.AddAuthorizationBuilder()
//	.SetDefaultPolicy(new AuthorizationPolicyBuilder()
//		.RequireAuthenticatedUser()
//		.AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme)
//		.Build()
//	)
//	.AddPolicy("Api", new AuthorizationPolicyBuilder()
//		.RequireAuthenticatedUser()
//		.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//		.Build()
//	);

services.AddApiClientForUser<IServicesApiV3Client, ServicesApiV3Client>("ApiV3", o => configuration.Bind("Apis:ApiV3", o));

services.AddSwagger(configuration, "Swagger", "Environment");

var app = builder.Build();

app.UseRouting();
app.UseAuthentication().UseAuthorization();

app.MapDefaultControllerRoute();

app.UseSwaggerWithUI(configuration, "Swagger", "Environment");

app.Run();
