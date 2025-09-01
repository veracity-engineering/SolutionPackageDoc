using Dapr.Client;
using DNV.Dapr.FeatureManagement;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;


var configStoreName = "configstore";

services.AddDaprClient();
services.AddDaprFeatureManagement(o =>
{
	o.ComponentName = configStoreName;
	o.KeyPrefix = "ff-";
});

var app = builder.Build();

app.MapGet("/ff-all", async (DaprClient client) =>
{
	var result = await client.GetConfiguration(configStoreName, []);
	return result.Items;
});

app.MapGet("/ff-enabled", (IFeatureManager fm) =>
{
	return fm.IsEnabledAsync("enabled");
});

app.MapGet("/ff-gate", () => "ff-gate is enabled")
	.WithFeatureGate("gate");

// should return 404
app.MapGet("/ff-unknown", () => "ff-unknown is enabled")
	.WithFeatureGate("unknown");

app.Run();
