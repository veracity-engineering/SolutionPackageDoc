using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DNV.Web.Swagger;

/// <summary>
/// Represents a middleware for proxying token requests to a remote OAuth2 endpoint.
/// </summary>
internal class TokenEndpointProxyMiddleware
{
	public const string ProxyClientName = "SwaggerClientProxy";

	/// <summary>
	/// A relative Uri representing the token endpoint used by this middleware.
	/// </summary>
	public static readonly Uri TokenEndpointProxyPath = new("/swagger/token", UriKind.Relative);

	private readonly RequestDelegate _next;
	private readonly Uri _remoteTokenEndpoint;

	/// <summary>
	/// Initializes an instance of <see cref="TokenEndpointProxyMiddleware"/>.
	/// </summary>
	/// <param name="next">The next middleware in the pipeline.</param>
	/// <param name="remoteTokenEndpoint">A URI pointing to the remote token endpoint.</param>
	public TokenEndpointProxyMiddleware(RequestDelegate next, Uri remoteTokenEndpoint)
	{
		_next = next;
		_remoteTokenEndpoint = remoteTokenEndpoint;
	}

	/// <summary>
	/// Invokes the middleware.
	/// </summary>
	/// <param name="httpContext">An object containing information about the current request.</param>
	/// <returns>A task that represents the completion of request processing.</returns> 
	public async Task InvokeAsync(HttpContext httpContext)
	{
		var (request, response) = (httpContext.Request, httpContext.Response);

		if (request.Path.StartsWithSegments(TokenEndpointProxyPath.OriginalString)
			&& request.Method == HttpMethods.Post
		)
		{
			var httpClient = httpContext.RequestServices.GetService<IHttpClientFactory>()?.CreateClient(ProxyClientName)
				?? new HttpClient();
			var requestMessage = CreateRequestMessage(request, _remoteTokenEndpoint);
			var responseMessage = await httpClient.SendAsync(requestMessage);

			var body = await responseMessage.Content.ReadAsStringAsync();
			response.StatusCode = (int)responseMessage.StatusCode;
			response.Headers.ContentLength = body.Length;
			response.Headers["Content-Type"] = "application/json; charset=utf-8";
			await response.WriteAsync(body);
			return;
		}

		await _next(httpContext);

		static HttpRequestMessage CreateRequestMessage(HttpRequest originalRequest, Uri endpoint)
		{
			var authorizationValue = originalRequest.Headers[HeaderNames.Authorization].ToString();
			var basicKey = authorizationValue[6..];
			var clientIdSecret = Base64UrlEncoder.Decode(basicKey).Split(':');

			var form = originalRequest.Form.ToDictionary(i => i.Key, i => i.Value.ToString());
			form["client_id"] = clientIdSecret[0];
			form["client_secret"] = clientIdSecret[1];
			var content = new FormUrlEncodedContent(form);

			var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
			requestMessage.Headers.Add(HeaderNames.Authorization, authorizationValue);
			return requestMessage;
		}
	}
}
