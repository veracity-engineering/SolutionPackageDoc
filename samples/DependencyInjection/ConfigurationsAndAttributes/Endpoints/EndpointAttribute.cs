namespace Examples.ConfigurationsAndAttributes.Endpoints;

[AttributeUsage(AttributeTargets.Class)]
public class EndpointAttribute : Attribute
{
	public EndpointAttribute(string feature, string baseRoute)
	{
		Feature = feature;
		BaseRoute = baseRoute;
	}

	public string Feature { get; }
	public string BaseRoute { get; }
}