namespace Examples.ConfigurationsAndAttributes.Features.SomeOtherFeature.Things;

public abstract class ThingBase : ICanDoSomething
{
	private readonly ILogger _logger;

	public ThingBase(ILogger logger)
	{
		_logger = logger;
	}

	public void DoSomething()
	{
		_logger.LogInformation("{ThingName} is doing something", GetType().Name);
	}
}