using System.Diagnostics;
using DNV.DependencyInjection.Abstractions;

namespace Examples.ConfigurationsAndAttributes.Features.SomeOtherFeature.Things;

[DisableAutoRegistration]
public class LoggingProxy : ICanDoSomething
{
	private readonly ICanDoSomething _doer;
	private readonly ILogger<LoggingProxy> _logger;

	public LoggingProxy(ICanDoSomething doer, ILogger<LoggingProxy> logger)
	{
		_doer = doer;
		_logger = logger;
	}
	public void DoSomething()
	{
		_logger.LogInformation("Starting timer");
		var watch = new Stopwatch();
		_doer.DoSomething();
		_logger.LogInformation("Elapsed time: {Elapsed}", watch.Elapsed);
	}
}