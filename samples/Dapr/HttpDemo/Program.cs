using DNV.Dapr.Common;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var mockServer = "https://cuteribs.requestcatcher.com";
var clientName = "DAPR_CLIENT";

services.AddHttpClient(
	clientName, 
	x =>
	{
		x.DefaultRequestHeaders.TryAddWithoutValidation(
			"InjectedHeader", "================= Your HttpClient is injected ================="
		);
		Console.WriteLine("================= Your HttpClient is injected =================");
	})
	.AddHttpMessageHandler(() => new ServiceInvocationHandler(mockServer, 3501))
	.AddHttpMessageHandler(() => new TestHandler());

var app = builder.Build();

app.MapGet("/dogs", (IHttpClientFactory factory) =>
{
	var req = new HttpRequestMessage(HttpMethod.Get, "http://fake.example.com/dogs");
	return factory.CreateClient(clientName).SendAsync(req);
});

app.MapPost("/dogs", (IHttpClientFactory factory, [FromBody] object breed) =>
{
	var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/dogs")
	{
		Content = JsonContent.Create(breed)
	};
	return factory.CreateClient(clientName).SendAsync(req);
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