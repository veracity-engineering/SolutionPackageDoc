using DNV.DependencyInjection.Abstractions;

namespace Examples.ConfigurationsAndAttributes.Endpoints;

public class EndpointsServiceAdder : ICanAddServicesForTypesWith<EndpointAttribute>
{
	public void AddServiceFor(Type type, EndpointAttribute attribute, IServiceCollection services)
	{
		services.AddSingleton(new EndpointDefinition(type, attribute.Feature, attribute.BaseRoute));
	}
}