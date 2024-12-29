using DNV.OAuth.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;
using System.Net.Http;

namespace DNV.OAuth.ApiClient;

public static class Extensions
{
	public static IServiceCollection AddApiClientForUser<TClient, TImplementation>(
		this IServiceCollection services,
		string apiName,
		Action<ApiClientOptions> configureOptions,
		Action<IServiceProvider, HttpClient>? configureClient = null
	) where TClient : class
		where TImplementation : class, TClient
		=> services.AddApiClient<TClient, TImplementation>(
			apiName,
			ApiClientAccessorType.User,
			configureOptions,
			configureClient
		);

	public static IServiceCollection AddApiClientForApp<TClient, TImplementation>(
		this IServiceCollection services,
		string apiName,
		Action<ApiClientOptions> configureOptions,
		Action<IServiceProvider, HttpClient>? configureClient = null
	) where TClient : class
		where TImplementation : class, TClient
		=> services.AddApiClient<TClient, TImplementation>(
			apiName,
			ApiClientAccessorType.App,
			configureOptions,
			configureClient
		);

	public static IServiceCollection AddApiClient<TClient, TImplementation>(
		this IServiceCollection services,
		string apiName,
		ApiClientAccessorType accessorType,
		Action<ApiClientOptions> configureOptions,
		Action<IServiceProvider, HttpClient>? configureClient = null
	) where TClient : class
		where TImplementation : class, TClient
	{
		var options = new ApiClientOptions();
		configureOptions(options);

		var builder = configureClient == null ?
			services.AddHttpClient(apiName) :
			services.AddHttpClient(apiName, configureClient);

		builder.AddHttpMessageHandler(sp =>
		{
			if (string.IsNullOrWhiteSpace(options.Scope))
			{
				var oauthOptions = sp.GetRequiredService<VeracityOAuthOptions>()
					.ThrowIfNull(new InvalidOperationException($"Cannot found Scope for Api {apiName}"));
				options.Scope = accessorType == ApiClientAccessorType.User
					? oauthOptions.DefaultUserScope
					: oauthOptions.DefaultAppScope;
			}

			var tokenAcquisition = sp.GetRequiredService<ITokenAcquisition>();
			return new ApiClientTokenHandler(tokenAcquisition, accessorType, options);
		});

		// this part could be extracted to a Func<> parameter for user to customize the client
		services.AddScoped<TClient>(sp =>
		{
			var httpClient = sp.GetRequiredService<IHttpClientFactory>()
				.CreateClient(apiName);

			if (options.BaseUrl != null) httpClient.BaseAddress = new(options.BaseUrl);

			return ActivatorUtilities.CreateInstance<TImplementation>(sp, httpClient);
		});

		return services;
	}
}
