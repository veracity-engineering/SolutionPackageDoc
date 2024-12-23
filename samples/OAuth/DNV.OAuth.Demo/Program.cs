using DNV.ApiClients.Veracity.Identity.ServicesApiV3;
using DNV.ApiClients.Veracity.Identity.ServicesApiV3.Interfaces;
using DNV.OAuth.ApiClient;
using DNV.OAuth.Web;
using DNV.Web.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllersWithViews();

var authBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
authBuilder.AddVeracityWebApp(configuration, "OAuth", "Environment")
	.EnableTokenAcquisitionToCallDownstreamApi()
	.AddDistributedTokenCaches();

authBuilder.AddVeracityWebApi(configuration, "OAuth", "Environment");

services.AddAuthorizationBuilder()
	.SetDefaultPolicy(new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme)
		.Build()
	)
	.AddPolicy("Api", new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
		.Build()
	);

services.AddApiClientForUser<IServicesApiV3Client, ServicesApiV3Client>("ApiV3", o => configuration.Bind("Apis:ApiV3", o));

services.AddSwagger(o => configuration.Bind("SwaggerOptions", o));

var app = builder.Build();

app.UseRouting();
app.UseAuthentication().UseAuthorization();

app.MapDefaultControllerRoute();

app.UseSwaggerWithUI(o => configuration.Bind("SwaggerOptions", o));

app.Run();
