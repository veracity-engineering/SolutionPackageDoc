using Dapr.Client;
using DNV.Dapr.PubSub;
using DNV.Dapr.PubSub.Abstractions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var clientName = "PUBSUB_CLIENT";

services.AddHttpClient(clientName)
	.AddHttpMessageHandler(() => new PubSubHandler());

services.AddDaprPubSubClient(
	clientName,
	o =>
	{
		o.PubSubName = "pubsub";
		o.TopicName = "testtopic";
	},
	o => o.HttpPort = 3501
);

var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapPost("/httppub", async (IDaprPubSubClientFactory factory, [FromBody] object message) =>
{
	var metadata = new Dictionary<string, string>
	{
		{ "pubMeta", "pub value" }
	};
	var client = factory.CreateClient(clientName);
	await client.PublishAsync(message, metadata);
});

app.MapPost("/httppubs", async (IDaprPubSubClientFactory factory, [FromBody] IEnumerable<object> messages) =>
{
	var metadata = new Dictionary<string, string>
	{
		{ "pubMeta", "pub value" }
	};
	var client = factory.CreateClient(clientName);
	await client.BulkPublishAsync(messages, metadata);
});

app.MapPost("/sdkpub", async ([FromBody] object message) =>
{
	var metadata = new Dictionary<string, string>
	{
		{ "pubMeta", "pub value" }
	};
	var client = new DaprClientBuilder()
		.UseHttpEndpoint("http://localhost:3501")
		.Build();
	await client.PublishEventAsync("pubsub", "testtopic", message, metadata);
});

app.MapPost("/sub", (HttpContext context, object message) =>
	{
		Console.WriteLine($"============================= Receive message =============================");

		var headers = context.Request.Headers;

		foreach (var header in headers)
		{
			Console.WriteLine($"  >>  {header.Key}: {header.Value}");
		}

		Console.WriteLine(message);
		return Results.Ok();
	})
	.WithTopic("pubsub", "testtopic");

app.Run();


public class PubSubHandler : DelegatingHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var uriBuilder = new UriBuilder(request.RequestUri!);
		var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
		query["metadata.injectedMeta"] = "injected value";
		uriBuilder.Query = query.ToString();
		request.RequestUri = uriBuilder.Uri;

		return base.SendAsync(request, cancellationToken);
	}
}