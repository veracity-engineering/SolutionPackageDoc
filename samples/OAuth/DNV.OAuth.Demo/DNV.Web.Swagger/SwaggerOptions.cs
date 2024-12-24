using DNV.OAuth.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DNV.Web.Swagger;

/// <summary>
/// Represents the options for configuring Swagger.
/// </summary>
public class SwaggerOptions
{
	/// <summary>
	/// Gets or sets a value indicating whether Swagger is enabled. Defaults to false.
	/// </summary>
	public bool Enabled { get; set; }

	/// <summary>
	/// Gets or sets the title of the Swagger document. Defaults to "Swagger UI".
	/// </summary>
	public string? DocumentTitle { get; set; } = "Swagger UI";

	/// <summary>
	/// Gets or sets a value indicating whether syntax highlighting should be used in Swagger. Defaults to false to avoid performance issue of Swagger UI for large json content highlighting.
	/// </summary>
	public bool SyntaxHighlight { get; set; }

	/// <summary>
	/// Combines all API versions into a single Swagger document. Defaults to false.
	/// </summary>
	public bool MonoVersioning { get; set; }

	/// <summary>
	/// Gets or sets the API versioning information.
	/// </summary>
	public IDictionary<string, OpenApiInfo>? Versions { get; set; }

	/// <summary>
	/// Gets or sets an action for additional configuration on SwaggerGen.
	/// Only used in <see cref="SwaggerExtensions.AddSwagger(IServiceCollection, Action{SwaggerOptions}?)" /> extension method.
	/// </summary>
	public Action<SwaggerGenOptions>? PostGenConfigure { get; set; }

	/// <summary>
	/// Gets or sets an action for additional configuration on SwaggerUI.
	/// Only used in <see cref="SwaggerExtensions.UseSwaggerWithUI(IApplicationBuilder, Action{SwaggerOptions}?)" /> extension method.
	/// </summary>
	public Action<SwaggerUIOptions>? PostUIConfigure { get; set; }

	/// <summary>
	/// Gets or sets OAuth2 settings
	/// </summary>
	public Authentication? OAuth { get; set; }

	public class Authentication
	{
		/// <summary>
		/// Gets or sets the client ID for Swagger OAuth2 authentication.
		/// </summary>
		public string? ClientId { get; set; }

		/// <summary>
		/// Gets or sets the client secret for Swagger OAuth2 authentication.
		/// </summary>
		public string? ClientSecret { get; set; }

		/// <summary>
		/// Gets or sets the implicit flow for OAuth2 authentication.
		/// </summary>
		public AuthFlow? ImplicitFlow { get; set; }

		/// <summary>
		/// Gets or sets the authorization code flow for OAuth2 authentication.
		/// </summary>
		public AuthFlow? AuthCodeFlow { get; set; }

		/// <summary>
		/// Gets or sets the client credential flow for OAuth2 authentication.
		/// </summary>
		public AuthFlow? ClientCredsFlow { get; set; }

		public OpenApiOAuthFlows ToOpenApiOAuthFlows()
		{
			var flows = new OpenApiOAuthFlows();

			if (this.ImplicitFlow != null)
			{
				flows.Implicit = new()
				{
					AuthorizationUrl = this.ImplicitFlow.AuthorizationUrl.ThrowIfNull(),
					Scopes = this.ImplicitFlow.Scopes.ToDictionary(p => p.Value, p => p.Key)
				};
			}

			if (this.AuthCodeFlow != null)
			{
				flows.AuthorizationCode = new()
				{
					AuthorizationUrl = this.AuthCodeFlow.AuthorizationUrl.ThrowIfNull(),
					TokenUrl = this.AuthCodeFlow.TokenUrl.ThrowIfNull(),
					Scopes = this.AuthCodeFlow.Scopes.ToDictionary(p => p.Value, p => p.Key)
				};
			}

			if (this.ClientCredsFlow != null)
			{
				flows.ClientCredentials = new()
				{
					TokenUrl = TokenEndpointProxyMiddleware.TokenEndpointProxyPath,
					Scopes = this.ClientCredsFlow.Scopes.ToDictionary(p => p.Value, p => p.Key)
				};
			}

			return flows;
		}
	}

	public class AuthFlow
	{
		/// <summary>
		/// REQUIRED. The authorization URL to be used for this flow.
		/// Applies to implicit and authorizationCode OAuthFlow.
		/// </summary>
		public Uri? AuthorizationUrl { get; set; }

		/// <summary>
		/// REQUIRED. The token URL to be used for this flow.
		/// Applies to password, clientCredentials, and authorizationCode OAuthFlow.
		/// </summary>
		public Uri? TokenUrl { get; set; }

		/// <summary>
		/// The URL to be used for obtaining refresh tokens.
		/// </summary>
		public Uri? RefreshUrl { get; set; }

		/// <summary>
		/// REQUIRED. A map between the scope name and a short description for it.
		/// </summary>
		public IDictionary<string, string> Scopes { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the resource Ids as audiences.
		/// Only used when <see cref="Scopes" /> is empty.
		/// </summary>
		public IDictionary<string, string> Audiences { get; set; } = new Dictionary<string, string>();
	}
}