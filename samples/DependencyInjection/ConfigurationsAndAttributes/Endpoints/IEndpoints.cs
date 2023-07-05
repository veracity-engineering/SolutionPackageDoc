namespace Examples.ConfigurationsAndAttributes.Endpoints;

public interface IEndpoints
{
	IEnumerable<EndpointDefinition> All { get; }
	IDictionary<string, IEnumerable<EndpointDefinition>> PerFeature { get; }
	IEnumerable<EndpointDefinition> GetForFeature(string feature);
}