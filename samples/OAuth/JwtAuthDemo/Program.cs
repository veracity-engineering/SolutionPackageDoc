using DNV.OAuth.Web;
using DNV.Web.Swagger;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();

var jwtOptions = configuration.GetSection("JwtOptions").GetChildren();
services.AddAuthentication("Bearer")
	.AddJwt(jwtOptions);

services.AddSwagger(o => configuration.Bind("Swagger", o));

var app = builder.Build();

await app.LoadJwtConfiguration(jwtOptions.Select(x => x.Key));

app.UseSwaggerWithUI(o => configuration.Bind("Swagger", o));

app.UseAuthentication();

app.MapControllers();

await app.RunAsync();
