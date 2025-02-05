
using DNV.OAuth.Web;
using DNV.Web.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var oidcOptions = configuration.GetSection("OAuth").Get<OidcOptions>();

services.AddOidc(oidcOptions)
	.AddJwt(configuration.GetSection("JwtOptions").GetChildren());

services.AddControllersWithViews();

// add swagger generation and swagger UI with authentication feature
services.AddSwagger(o => configuration.Bind("SwaggerOptions", o));






var app = builder.Build();


app.UseDeveloperExceptionPage();

app.UseHttpsRedirection().UseRouting();
app.UseAuthentication().UseAuthorization();

app.MapDefaultControllerRoute();


app.UseSwaggerWithUI(o => configuration.Bind("SwaggerOptions", o));


app.Run();