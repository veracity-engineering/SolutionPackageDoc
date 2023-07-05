using Examples.ConfigurationsAndAttributes.Features.SomeOtherFeature.Things;

namespace Examples.ConfigurationsAndAttributes.Features.SomeOtherFeature;

public class SomeOtherFeature : ISomeOtherFeature
{
	private readonly IEnumerable<ICanDoSomething> _doers;

	public SomeOtherFeature(IEnumerable<ICanDoSomething> doers)
	{
		_doers = doers;
	}
	
	public Task DoSomething()
	{
		foreach (var doer in _doers)
		{
			doer.DoSomething();
		}

		return Task.CompletedTask;
	}
}