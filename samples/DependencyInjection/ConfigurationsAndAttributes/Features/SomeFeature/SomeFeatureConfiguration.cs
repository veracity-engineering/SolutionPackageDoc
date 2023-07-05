using DNV.DependencyInjection.Configuration;

namespace Examples.ConfigurationsAndAttributes.Features.SomeFeature;

[Configuration("SomeFeature")]
public record SomeFeatureConfiguration(SomeFeatureType Version, string? CustomPayload);