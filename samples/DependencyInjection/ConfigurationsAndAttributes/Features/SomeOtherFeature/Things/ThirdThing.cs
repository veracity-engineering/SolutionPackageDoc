using DNV.DependencyInjection.Abstractions;

namespace Examples.ConfigurationsAndAttributes.Features.SomeOtherFeature.Things;

[LoggingProxy]
[DisableAutoRegistration]
public class ThirdThing : ThingBase
{

	public ThirdThing(ILogger<ThirdThing> logger) : base(logger)
	{
	}
}