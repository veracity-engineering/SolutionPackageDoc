using DNV.DependencyInjection.Abstractions;

namespace Examples.ConfigurationsAndAttributes.Features.SomeFeature;

[DisableAutoRegistration]
public class FastSomeFeature : ISomeFeature
{
	private readonly string _payload;

	public FastSomeFeature(string? customPayload)
	{
		_payload = customPayload ?? "Default payload";
	}
	
	public async Task<string> DoSomething()
	{
		await Task.Delay(100);
		return "Fast returns: " + _payload;
	}
}