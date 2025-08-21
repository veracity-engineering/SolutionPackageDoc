using DNV.Dapr.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var mockServer = "https://cuteribs.requestcatcher.com";
var clientName = nameof(DefaultDaprHttpClient);// "DAPR_CLIENT";

services.AddHttpClient<DefaultDaprHttpClient>(clientName, x =>
{
	x.DefaultRequestHeaders.TryAddWithoutValidation("================= Your HttpClient is injected =================", "Injected");
	Console.WriteLine("================= Your HttpClient is injected =================");
}).AddHttpMessageHandler(() => new TestHandler());

services.AddTransient<DaprHttpClient>(sp =>
{
	var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
	return new DefaultDaprHttpClient(httpClientFactory.CreateClient(clientName), new() { HttpPort = 3500 });
});

var app = builder.Build();

app.MapGet("/dogs", (DaprHttpClient daprHttpClient) =>
{
	return daprHttpClient.InvokeMethodAsync<string>("GET", mockServer, "dogs");
});

app.MapPost("/dogs", (DaprHttpClient daprHttpClient, [FromBody] object breed) =>
{
	return daprHttpClient.InvokeMethodAsync<string>("POST", mockServer, "dogs", breed);
});

app.MapPost("/pubDog", (DaprHttpClient daprHttpClient, [FromBody] object dog) =>
{
	var metadata = new Dictionary<string, string>
	{
		{ "pubMeta", "pub value" }
	};
	return daprHttpClient.PublishEventAsync("pubsub", "dogtopic", dog, metadata);
});

app.Run();

public class TestHandler : DelegatingHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		request.Headers.TryAddWithoutValidation("dapr-client", "default dapr http client");
		var uri = request.RequestUri?.OriginalString ?? "";
		uri += (uri.Contains('?') ? "&" : "?") + "param1=abc";
		request.RequestUri = new(uri);
		return base.SendAsync(request, cancellationToken);
	}
}