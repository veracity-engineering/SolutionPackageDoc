using Microsoft.AspNetCore.Mvc;

namespace Examples.ConfigurationsAndAttributes.Endpoints;

[Endpoint("Endpoints", "/endpoints")]
[Route("[controller]")]
public class EndpointsController : ControllerBase
{
	private readonly IEndpoints _endpoints;

	public EndpointsController(IEndpoints endpoints)
	{
		_endpoints = endpoints;
	}

	[HttpGet]
	public Task<IEnumerable<string>> All() =>
		Task.FromResult(_endpoints.All.Select(_ => _.Feature));
	
	[HttpGet("[controller]/{featureName}")]
	public Task<IEnumerable<string>> ForFeature(string featureName) =>
		Task.FromResult(_endpoints.GetForFeature(featureName).Select(_ => _.BaseRoute));
}