using DNV.DependencyInjection.Configuration;
using DNV.DependencyInjection.Microsoft;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDnvMicrosoftDependencyInjection("ConfigurationsAndAttributes",
	_ => _.DefaultLifetime = ServiceLifetime.Scoped);
builder.Host.UseDNVConfigurations();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();