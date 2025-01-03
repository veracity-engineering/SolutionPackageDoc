using DNV.Monitoring.HealthChecks.ServiceHealthCheck;
using DNV.Monitoring.HealthChecks.VeracityStatus;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Begin: For health check
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<VeracityStatusHealthCheckOptions>(configuration.GetSection(nameof(VeracityStatusHealthCheckOptions)));
var serviceProvider = builder.Services.BuildServiceProvider();
var veracityStatusHealthCheckOptions = serviceProvider.GetService<IOptions<VeracityStatusHealthCheckOptions>>().Value;
builder.Services.AddHealthChecks()
    .AddServiceHealthCheck(new ServiceHealthCheckOptions { Uri = new Uri("https://localhost:7204/health") }, "Child Status");
// End: For health check

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

app.MapVeracityHealthChecks("/health", veracityStatusHealthCheckOptions);

app.Run();
