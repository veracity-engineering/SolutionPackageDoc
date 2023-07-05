using Examples.ConfigurationsAndAttributes.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace Examples.ConfigurationsAndAttributes.Features.SomeFeature;

[Endpoint("SomeFeature", "/somefeature")]
[Route("[controller]")]
public class SomeFeatureController : ControllerBase
{
	private readonly ISomeFeature _someFeature;

	public SomeFeatureController(ISomeFeature someFeature)
	{
		_someFeature = someFeature;
	}

	[HttpGet]
	public Task<string> Get() => _someFeature.DoSomething();
}