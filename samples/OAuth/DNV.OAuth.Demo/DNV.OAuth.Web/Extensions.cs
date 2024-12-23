using DNV.OAuth.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;

namespace DNV.OAuth.Web;

public static class Extensions
{
	#region AddVeracityWebApp
	public static MicrosoftIdentityWebAppAuthenticationBuilder AddVeracityWebApp(
	this IServiceCollection services,
	IConfigurationRoot configuration,
	string configSectionName = Constants.AzureAd,
	string environmentKey = "Environment"
) => services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddVeracityWebApp(configuration, configSectionName, environmentKey);

	public static MicrosoftIdentityWebAppAuthenticationBuilder AddVeracityWebApp(
		this AuthenticationBuilder builder,
		IConfigurationRoot configuration,
		string configSectionName = Constants.AzureAd,
		string environmentKey = "Environment"
	)
	{
		var configSection = configuration.GetSection(configSectionName);
		var environment = configuration.GetEnum<VeracityEnvironment>(environmentKey);
		return builder.AddVeracityWebApp(configSection, environment);
	}

	public static MicrosoftIdentityWebAppAuthenticationBuilder AddVeracityWebApp(
		this AuthenticationBuilder builder,
		IConfigurationSection configuration,
		VeracityEnvironment environment = VeracityEnvironment.Production
	) => builder.AddVeracityWebApp(configuration.Bind, environment);

	public static MicrosoftIdentityWebAppAuthenticationBuilder AddVeracityWebApp(
		this AuthenticationBuilder builder,
		Action<MicrosoftIdentityOptions> configureMicrosoftIdentityOptions,
		VeracityEnvironment environment = VeracityEnvironment.Production,
		Action<CookieAuthenticationOptions>? configureCookieAuthenticationOptions = null
	)
	{
		var oauthOptions = VeracityOAuthOptions.Get(environment);
		builder.Services.AddSingleton<VeracityOAuthOptions>(oauthOptions);
		return builder.AddMicrosoftIdentityWebApp(
			o => oauthOptions.ConfigureIdentityOptions(o, configureMicrosoftIdentityOptions),
			configureCookieAuthenticationOptions
		);
	}
	#endregion

	#region AddVeracityWebApi
	public static MicrosoftIdentityWebApiAuthenticationBuilder AddVeracityWebApi(
		this IServiceCollection services,
		IConfigurationRoot configuration,
		string configSectionName = Constants.AzureAd,
		string environmentKey = "Environment",
		string scheme = JwtBearerDefaults.AuthenticationScheme
	) => services.AddAuthentication(scheme)
		.AddVeracityWebApi(configuration, configSectionName, environmentKey, scheme);

	public static MicrosoftIdentityWebApiAuthenticationBuilder AddVeracityWebApi(
		this AuthenticationBuilder builder,
		IConfigurationRoot configuration,
		string configSectionName = Constants.AzureAd,
		string environmentKey = "Environment",
		string scheme = JwtBearerDefaults.AuthenticationScheme
	)
	{
		var configSection = configuration.GetSection(configSectionName);
		var environment = configuration.GetEnum<VeracityEnvironment>(environmentKey);
		return builder.AddVeracityWebApi(configSection, environment, scheme);
	}

	public static MicrosoftIdentityWebApiAuthenticationBuilder AddVeracityWebApi(
		this AuthenticationBuilder builder,
		IConfigurationSection configuration,
		VeracityEnvironment environment = VeracityEnvironment.Production,
		string scheme = JwtBearerDefaults.AuthenticationScheme
	) => builder.AddVeracityWebApi(configuration.Bind, configuration.Bind, environment, scheme);

	public static MicrosoftIdentityWebApiAuthenticationBuilder AddVeracityWebApi(
		this AuthenticationBuilder builder,
		Action<JwtBearerOptions> configureJwtBearerOptions,
		Action<MicrosoftIdentityOptions> configureMicrosoftIdentityOptions,
		VeracityEnvironment environment = VeracityEnvironment.Production,
		string scheme = JwtBearerDefaults.AuthenticationScheme
	)
	{
		var oauthOptions = VeracityOAuthOptions.Get(environment);
		builder.Services.AddSingleton<VeracityOAuthOptions>(oauthOptions);
		return builder.AddMicrosoftIdentityWebApi(
			o => oauthOptions.ConfigureJwtOptions(o, configureJwtBearerOptions),
			o => oauthOptions.ConfigureIdentityOptions(o, configureMicrosoftIdentityOptions),
			scheme
		);
	}

	public static void ConfigureIdentityOptions(
		this VeracityOAuthOptions oauthOptions,
		MicrosoftIdentityOptions options,
		Action<MicrosoftIdentityOptions>? configureNext = null
	)
	{
		options.Instance ??= oauthOptions.Instance;
		options.Domain ??= oauthOptions.Domain;
		options.TenantId ??= oauthOptions.TenantId;
		options.SignUpSignInPolicyId ??= oauthOptions.UserFlow;
		options.Scope.Add(oauthOptions.DefaultUserScope);

		configureNext?.Invoke(options);
	}

	public static void ConfigureJwtOptions(
		this VeracityOAuthOptions oauthOptions,
		JwtBearerOptions options,
		Action<JwtBearerOptions>? configureNext = null
	)
	{
		//options.Authority ??= oauthOptions.Authority;

		configureNext?.Invoke(options);
	}
	#endregion

	public static T GetEnum<T>(this IConfiguration configuration, string key)
		where T : struct, Enum
	{
		var value = configuration[key];

		if (Enum.TryParse<T>(value, out var result))
		{
			return result;
		}

		throw new ArgumentException($"Invalid value for {key}: {value}");
	}
}
