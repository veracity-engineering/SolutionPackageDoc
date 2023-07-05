using DNV.DependencyInjection.Abstractions.Lifetime;

namespace Examples.ConfigurationsAndAttributes.Endpoints;

[WithLifetime(ServiceLifetime.Singleton)]
public class Endpoints : IEndpoints
{
	public Endpoints(IEnumerable<EndpointDefinition> endpointDefinitions)
	{
		All = endpointDefinitions;
		PerFeature = endpointDefinitions.GroupBy(_ => _.Feature, _ => _).ToDictionary(_ => _.Key, _ => _.AsEnumerable());
	}
	public IEnumerable<EndpointDefinition> All { get; }
	public IDictionary<string, IEnumerable<EndpointDefinition>> PerFeature { get; }

	public IEnumerable<EndpointDefinition> GetForFeature(string feature)
	{
		if (!PerFeature.TryGetValue(feature, out var definitions))
		{
			definitions = Enumerable.Empty<EndpointDefinition>();
		}

		return definitions;
	}
}