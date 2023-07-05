using DNV.DependencyInjection.Abstractions;

namespace Examples.ConfigurationsAndAttributes.Features.SomeFeature;

[DisableAutoRegistration]
public class SlowSomeFeature : ISomeFeature
{
	private readonly string _payload;

	public SlowSomeFeature(string? customPayload)
	{
		_payload = customPayload ?? "Default payload";
	}
	
	public async Task<string> DoSomething()
	{
		await Task.Delay(1000);
		return "Slow returns: " + _payload;
	}
}