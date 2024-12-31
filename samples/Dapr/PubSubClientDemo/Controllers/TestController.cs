using DNV.Dapr.PubSub;
using Microsoft.AspNetCore.Mvc;

namespace PubSubClientDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
	private readonly ILogger<TestController> _logger;
	private readonly IDaprPubSubClientFactory _factory;

	public TestController(ILogger<TestController> logger, IDaprPubSubClientFactory factory)
	{
		_logger = logger;
		_factory = factory;
	}

	[HttpPost("pub1")]
	public async Task Pub1(string text)
	{
		_logger.LogWarning("Sending message: {message}", text);

		var message = new MessageEvent(Guid.NewGuid().ToString(), text);
		await _factory.CreateClient("pubsub1").PublishAsync(message);
	}

	[HttpPost("pub2")]
	public async Task Pub2(string text)
	{
		_logger.LogWarning("Sending message: {message}", text);

		var message = new MessageEvent(Guid.NewGuid().ToString(), text);
		await _factory.CreateClient("pubsub2").PublishAsync(message);
	}

	[HttpPost("sub1")]
	public void Sub1(MessageEvent message)
	{
		_logger.LogWarning("Message 1 received: {message}", message.Message);
	}

	[HttpPost("sub2")]
	public void Sub2(MessageEvent message)
	{
		_logger.LogWarning("Message 2 received: {message}", message.Message);
	}
}

public record MessageEvent(string MessageType, string Message);