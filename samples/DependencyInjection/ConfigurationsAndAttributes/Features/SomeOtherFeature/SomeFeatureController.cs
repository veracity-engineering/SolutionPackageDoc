using Examples.ConfigurationsAndAttributes.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace Examples.ConfigurationsAndAttributes.Features.SomeOtherFeature;

[Endpoint("SomeOtherFeature", "/someotherfeature")]
[Route("[controller]")]
public class SomeOtherFeatureController : ControllerBase
{
	private readonly ISomeOtherFeature _feature;

	public SomeOtherFeatureController(ISomeOtherFeature feature)
	{
		_feature = feature;
	}

	[HttpGet]
	public Task DoSomething() => _feature.DoSomething();
}