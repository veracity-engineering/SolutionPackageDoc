using DNV.OAuth.Common;
using DNV.Web.Swagger.Fixes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DNV.Web.Swagger;

/// <summary>
/// Contains extension methods for configuring Swagger.
/// </summary>
public static class SwaggerExtensions
{
	public static readonly string NullableKey = "x-nullable";
	public static readonly string FallbackXmlCommentsName = "Swagger.xml";
	public static readonly string MonoVersion = "mono";

	public static IServiceCollection AddSwagger(
		this IServiceCollection services,
		IConfigurationRoot configuration,
		string configSectionName = "Swagger",
		string environmentKey = "Environment"
	)
	{
		Enum.TryParse<VeracityEnvironment>(configuration[environmentKey], true, out var environment);

		return services.AddSwagger(
			o => configuration.GetSection(configSectionName).Bind(o),
			environment
		);
	}

	/// <summary>
	/// Adds Swagger services to the specified service collection.
	/// </summary>
	/// <param name="services">The service collection used in the startup of the application.</param>
	/// <param name="setupAction">The action used to configure specified options.</param>
	/// <param name="environment">The Veracity environment to use for OAuth2 configuration.</param>
	/// <returns>The updated service collection.</returns>
	public static IServiceCollection AddSwagger(
		this IServiceCollection services,
		Action<SwaggerOptions>? setupAction = null,
		VeracityEnvironment? environment = null
	)
	{
		var options = new SwaggerOptions();
		setupAction?.Invoke(options);

		// If Swagger is not enabled, just return the services as-is
		if (!options.Enabled) return services;

		var oauthOptions = environment.HasValue
			? VeracityOAuthOptions.Get(environment.Value)
			: null;

		ConfigureWithOAuthOptions(options, oauthOptions);

		if (options.OAuth?.ClientSecret != null)
		{
			// Prepare proxy client for client credentials
			services.AddHttpClient(TokenEndpointProxyMiddleware.ProxyClientName);
		}

		// Register the configuration of the Swagger service through the .AddSwaggerGen() method and the SetupSwaggerGenOptions() helper method
		return services.AddSwaggerGen(o => SetupSwaggerGenOptions(o, options, oauthOptions));

		static void ConfigureWithOAuthOptions(SwaggerOptions options, VeracityOAuthOptions? oauthOptions)
		{
			if (oauthOptions == null) return;

			var oauth = options.OAuth;

			if (oauth?.ImplicitFlow != null || oauth?.AuthCodeFlow != null || oauth?.ClientCredsFlow != null)
			{
				var endpointBaseUrl = $"{oauthOptions.Instance}/{oauthOptions.Domain}/{oauthOptions.UserFlow}/oauth2/v2.0";
				var authorizeEndpoint = new Uri($"{endpointBaseUrl}/authorize");
				var tokenEndpoint = new Uri($"{endpointBaseUrl}/token");

				SwaggerOptions.AuthFlow? flow;

				if ((flow = oauth.ImplicitFlow) != null)
				{
					flow.AuthorizationUrl ??= authorizeEndpoint;
					ApplyUserScopes(flow, oauthOptions);
				}

				if ((flow = oauth.AuthCodeFlow) != null)
				{
					flow.AuthorizationUrl ??= authorizeEndpoint;
					flow.TokenUrl ??= tokenEndpoint;
					ApplyUserScopes(flow, oauthOptions);
				}

				if ((flow = oauth.ClientCredsFlow) != null)
				{
					flow.TokenUrl ??= tokenEndpoint;
					ApplyAppScopes(flow, oauthOptions);
				}
			}
		}

		static void SetupSwaggerGenOptions(SwaggerGenOptions genOptions, SwaggerOptions options, VeracityOAuthOptions? oauthOptions)
		{
			if (options.MonoVersioning)
			{
				genOptions.DocInclusionPredicate((_, _) => true);
				genOptions.SwaggerDoc(MonoVersion, new()
				{
					Title = options.DocumentTitle,
					Version = MonoVersion
				});
			}
			else if (options.Versions != null)
			{
				// Loop through all API version information objects sent through the options object and add to the Swagger documentation
				foreach (var info in options.Versions)
				{
					genOptions.SwaggerDoc(info.Key, info.Value);
				}
			}
			else
			{
				genOptions.SwaggerDoc("v1", new() { Version = "v1" });
			}

			// Include XML comments in Swagger documentation for route descriptors
			var xmlFilename = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml");

			if (!File.Exists(xmlFilename))
			{
				xmlFilename = Path.Combine(AppContext.BaseDirectory, FallbackXmlCommentsName);
			}

			if (File.Exists(xmlFilename))
			{
				genOptions.IncludeXmlComments(xmlFilename);
			}

			if (options.OAuth != null)
			{
				// Set up the OAuth2 security scheme and HTTP/Generic security scheme
				SetupOAuth2SecurityScheme(genOptions, options, oauthOptions);
				SetupHttpSecurityScheme(genOptions);
			}

			genOptions.TagActionsBy(d =>
			{
				var actionDescriptor = d.ActionDescriptor;
				var tagsMetadata = actionDescriptor.EndpointMetadata?
					.OfType<ITagsMetadata>()
					.LastOrDefault();

				return tagsMetadata?.Tags.ToList()
					?? [actionDescriptor.RouteValues["controller"]];
			});

			// Customize OperationId in swagger.json 
			genOptions.CustomOperationIds(d =>
			{
				var actionDescriptor = d.ActionDescriptor;

				if (actionDescriptor is ControllerActionDescriptor cad)
				{
					return options.MonoVersioning && d.GroupName != null
						? $"{cad.ControllerName}{d.GroupName.ToUpper()}_{cad.ActionName}"
						: $"{cad.ControllerName}_{cad.ActionName}";
				}

				var name = actionDescriptor.AttributeRouteInfo?.Name
					?? actionDescriptor.EndpointMetadata?
						.OfType<IEndpointNameMetadata>()
						.LastOrDefault()?
						.EndpointName;

				return options.MonoVersioning && d.GroupName != null
					? $"{name}{d.GroupName.ToUpper()}"
					: name;
			});

			genOptions.OrderActionsBy(d => d.RelativePath);
			genOptions.SupportNonNullableReferenceTypes();

			// apply fixes
			genOptions.ParameterFilter<ParameterFilterFix>();
			genOptions.SchemaFilter<SchemaFilterFix>();
			genOptions.RequestBodyFilter<RequestBodyFilterFix>();

			options.PostGenConfigure?.Invoke(genOptions);
		}

		static void SetupOAuth2SecurityScheme(SwaggerGenOptions genOptions, SwaggerOptions options, VeracityOAuthOptions? oauthOptions)
		{
			var oauth = options.OAuth!;

			// Create a new instance of the OpenApiSecurityScheme that describes an OAuth2 authentication scheme
			var securityScheme = new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.OAuth2,
				Scheme = "OAuth2",
				Reference = new OpenApiReference { Id = "OAuth2", Type = ReferenceType.SecurityScheme }
			};

			securityScheme.Flows = oauth.ToOpenApiOAuthFlows();

			// Add the security definition and requirement for the flow to genOptions
			genOptions.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
			genOptions.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
		}

		static void SetupHttpSecurityScheme(SwaggerGenOptions genOptions)
		{
			// Create a new instance of the OpenApiSecurityScheme that describes an HTTP or generic token-based authentication scheme
			var securityScheme = new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer",
				In = ParameterLocation.Header,
				Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
			};
			// Add the defined httpScheme as a security definition to genOptions
			genOptions.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
			genOptions.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
		}

		static void ApplyUserScopes(SwaggerOptions.AuthFlow flow, VeracityOAuthOptions oauthOptions)
		{
			if (flow.Scopes.Any() != true)
			{
				if (flow.Audiences.Any() != true)
				{
					flow.Audiences.Add(new("Default", oauthOptions.DefaultUserScope));
				}

				flow.Scopes = flow.Audiences
					.ToDictionary(x => x.Key, x => oauthOptions.GetUserScope(x.Value.ToString()));
			}
		}

		static void ApplyAppScopes(SwaggerOptions.AuthFlow flow, VeracityOAuthOptions oauthOptions)
		{
			if (flow.Scopes.Any() != true)
			{
				if (flow.Audiences.Any() != true)
				{
					flow.Audiences.Add(new("Default", oauthOptions.DefaultAppScope));
				}

				flow.Scopes = flow.Audiences
					.ToDictionary(x => x.Key, x => oauthOptions.GetAppScope(x.Value.ToString()));
			}
		}
	}

	public static IApplicationBuilder UseSwaggerWithUI(
		this IApplicationBuilder app,
		IConfigurationRoot configuration,
		string configSectionName = "Swagger",
		string environmentKey = "Environment"
	)
	{
		Enum.TryParse<VeracityEnvironment>(configuration[environmentKey], true, out var environment);

		return app.UseSwaggerWithUI(
			o => configuration.GetSection(configSectionName).Bind(o),
			environment
		);
	}

	/// <summary>
	/// Extension method that adds Swagger middleware to the specified IApplicationBuilder with Swagger UI served from "/swagger/index.html".
	/// </summary>
	/// <param name="app">The IApplicationBuilder instance being extended</param>
	/// <param name="setupAction">Optional Lambda used to configure the SwaggerOptions</param>
	/// <param name="environment">Optional Veracity environment to use for OAuth2 configuration</param>
	/// <returns>The same instance of the IApplicationBuilder passed in, to allow for the usage of fluent methods.</returns>
	public static IApplicationBuilder UseSwaggerWithUI(
		this IApplicationBuilder app,
		Action<SwaggerOptions>? setupAction = null,
		VeracityEnvironment? environment = null
	)
	{
		var options = new SwaggerOptions();
		setupAction?.Invoke(options);

		if (!options.Enabled) return app;

		// Configure the Swagger endpoint and Swagger UI through middleware using .UseSwagger() and .UseSwaggerUI()
		app.UseSwagger()
			.UseSwaggerUI(o => SetupUIOptions(o, options));

		if (options.OAuth != null)
		{
			// Set up proxy middleware for handling token requests to the client credentials flow's token URL
			var tokenUrl = options.OAuth.ClientCredsFlow?.TokenUrl;

			if(tokenUrl == null && environment.HasValue)
			{
				var oauthOptions = VeracityOAuthOptions.Get(environment.Value);
				tokenUrl = new(GetOidcConfig(oauthOptions).TokenEndpoint);
			}

			if (tokenUrl != null)
			{
				app.UseMiddleware<TokenEndpointProxyMiddleware>(tokenUrl);
			}
		}

		return app;

		static void SetupUIOptions(SwaggerUIOptions uiOptions, SwaggerOptions options)
		{
			if (options.MonoVersioning)
			{
				uiOptions.SwaggerEndpoint($"{MonoVersion}/swagger.json", options.DocumentTitle);
			}
			else if (options.Versions != null)
			{
				foreach (var info in options.Versions)
				{
					uiOptions.SwaggerEndpoint($"{info.Key}/swagger.json", $"{info.Value.Title} {info.Key}");
				}
			}
			else
			{
				uiOptions.SwaggerEndpoint("v1/swagger.json", "v1");
			}

			// Configure other properties regarding the Swagger UI
			uiOptions.DocumentTitle = options.DocumentTitle;

			uiOptions.DisplayRequestDuration();

			if (options.OAuth != null)
			{
				uiOptions.OAuthClientId(options.OAuth.ClientId);
				uiOptions.OAuthClientSecret(options.OAuth.ClientSecret);
				uiOptions.OAuthUsePkce();
			}

			uiOptions.DisplayOperationId();
			uiOptions.EnableFilter();

			uiOptions.ConfigObject.AdditionalItems.Add("syntaxHighlight", options.SyntaxHighlight);

			options.PostUIConfigure?.Invoke(uiOptions);
		}
	}

	private static OpenIdConnectConfiguration GetOidcConfig(VeracityOAuthOptions options)
	{
		var endpointBaseUrl = $"{options.Instance}/{options.Domain}/{options.UserFlow}/oauth2/v2.0";
		return new()
		{
			AuthorizationEndpoint = $"{endpointBaseUrl}/authorize",
			TokenEndpoint = $"{endpointBaseUrl}/token"
		};
	}
}

