using Dapr.Client;
using DNV.Dapr.Common;
using DNV.Dapr.PubSub;
using DNV.Dapr.PubSub.Abstractions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var clientName = "PUBSUB_CLIENT";
var (pubsubName, topicName) = ("pubsub", "testtopic");

services.AddHttpClient(clientName)
	.AddHttpMessageHandler(() => new ExtraMetadataHandler());
//services.AddTransient<ExtraMetadataHandler>()
//	.AddHttpClient(clientName)
//	.AddHttpMessageHandler<ExtraMetadataHandler>();

services.AddDaprPubSubClient(clientName, o =>
	{
		o.HttpPort = 3501;
		o.PubSubName = "pubsub";
		o.TopicName = topicName;
	});

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
	await client.PublishEventAsync(pubsubName, topicName, message, metadata);
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
	.WithTopic(pubsubName, topicName);

app.Run();


public class ExtraMetadataHandler : DelegatingHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		request.RequestUri = request.RequestUri!
			.AppendQueryParam("injectedMeta", "injected value");
		return base.SendAsync(request, cancellationToken);
	}
}