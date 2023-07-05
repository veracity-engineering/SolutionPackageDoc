using DNV.DependencyInjection.Abstractions;
using Microsoft.Extensions.Options;

namespace Examples.ConfigurationsAndAttributes.Features.SomeFeature;

public class SomeFeatureServiceAdder : ICanAddServices
{
	public void AddTo(IServiceCollection builder)
	{
		builder.AddScoped<ISomeFeature>(serviceProvider =>
		{
			var config = serviceProvider.GetRequiredService<IOptionsSnapshot<SomeFeatureConfiguration>>().Value;
			return config.Version switch
			{
				SomeFeatureType.Fast => new FastSomeFeature(config.CustomPayload),
				SomeFeatureType.Slow => new SlowSomeFeature(config.CustomPayload),
				_ => throw new NotImplementedException($"No implementation of {nameof(ISomeFeature)} for version {config.Version}")
			};
		});
	}
}