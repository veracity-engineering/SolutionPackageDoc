using DNV.DependencyInjection.Abstractions;

namespace Examples.ConfigurationsAndAttributes.Features.SomeOtherFeature.Things;

public class LoggingProxyServices : ICanAddServicesForTypesWith<LoggingProxyAttribute>
{
	public void AddServiceFor(Type type, LoggingProxyAttribute attribute, IServiceCollection services)
	{
		services.AddScoped<ICanDoSomething>(_ =>
			new LoggingProxy((ActivatorUtilities.CreateInstance(_, type) as ICanDoSomething)!,
				_.GetRequiredService<ILogger<LoggingProxy>>()));
	}
}