using DNV.Monitoring.HealthChecks.VeracityStatus;
using DNV.Monitoring.HealthChecks.VeracityStatus.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.Configure<VeracityStatusHealthCheckOptions>(configuration.GetSection(nameof(VeracityStatusHealthCheckOptions)));
var serviceProvider = builder.Services.BuildServiceProvider();
var veracityStatusHealthCheckOptions = serviceProvider.GetService<IOptions<VeracityStatusHealthCheckOptions>>().Value;
builder.Services.AddHealthChecks().AddVeracityHealthCheck(veracityStatusHealthCheckOptions);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapVeracityHealthChecks("/health");

app.Run();
