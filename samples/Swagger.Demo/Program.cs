using DNV.Web.Swagger;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddProblemDetails()
	.AddProblemDetailsConventions();

services.AddSwagger(o => configuration.Bind("SwaggerOptions", o));

services.AddControllers()
	.AddJsonOptions(o =>
	{
		o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

		// * convert enum to string
		o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	})
	// required for JsonPatchDocument support
	.AddNewtonsoftJson();

services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseProblemDetails();

app.MapControllers();

app.UseSwaggerWithUI(o => configuration.Bind("SwaggerOptions", o));

app.Run();
