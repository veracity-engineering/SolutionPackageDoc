using DNV.Monitoring.HealthChecks.VeracityStatus;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "HealthCheckChild.xml");
    c.IncludeXmlComments(filePath);
    var filePath2 = Path.Combine(AppContext.BaseDirectory, "DNV.Monitoring.HealthChecks.VeracityStatus.xml");
    c.IncludeXmlComments(filePath2);
});

// Begin: For health check
builder.Services.AddHttpClient();
builder.Services.Configure<VeracityStatusHealthCheckOptions>(configuration.GetSection(nameof(VeracityStatusHealthCheckOptions)));
var serviceProvider = builder.Services.BuildServiceProvider();
var veracityStatusHealthCheckOptions = serviceProvider.GetService<IOptions<VeracityStatusHealthCheckOptions>>().Value;
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri("https://status.veracity.com/"), "Veracity Status");
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
