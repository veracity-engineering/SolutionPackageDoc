using DNV.Dapr.PubSub;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllers();
	//.AddDapr();             // for subscriber support with post endpoints

services.AddEndpointsApiExplorer()
	.AddSwaggerGen();

foreach (var section in configuration.GetSection("PubSubClients").GetChildren())
{
	services.AddDaprPubSubClient(section);
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// for subscriber support with post endpoints
app.UseCloudEvents();
//app.MapSubscribeHandler();

app.Run();
