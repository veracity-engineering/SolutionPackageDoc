using DNV.Web.Swagger;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddProblemDetails()
	.AddProblemDetailsConventions();

services.AddSwagger(o => configuration.Bind("SwaggerOptions", o))
	.AddSwaggerGenNewtonsoftSupport();

services.AddControllers()
	.AddNewtonsoftJson(o => o.SerializerSettings.Converters.Add(new StringEnumConverter()));

services.AddApiVersioning()
	.AddApiExplorer(o =>
	{
		o.GroupNameFormat = "'v'VVV";
		o.SubstituteApiVersionInUrl = true;
	});

var app = builder.Build();

app.UseProblemDetails();

app.MapDefaultControllerRoute();

app.UseSwaggerWithUI(o => configuration.Bind("SwaggerOptions", o));

app.Run();
