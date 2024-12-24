using DNV.OAuth.Common;
using Microsoft.Identity.Web;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DNV.OAuth.ApiClient;

public class ApiClientTokenHandler : DelegatingHandler
{
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly ApiClientAccessorType _accessorType;
	private readonly ApiClientOptions _options;

	public ApiClientTokenHandler(ITokenAcquisition tokenAcquisition, ApiClientAccessorType accessorType, ApiClientOptions options)
	{
		_tokenAcquisition = tokenAcquisition.ThrowIfNull(nameof(tokenAcquisition));
		_options = options.ThrowIfNull(nameof(options));
		options.Scope.ThrowIfNull(new ArgumentException("Scope is required"));
		_accessorType = accessorType;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var scope = _options.Scope!;
		var task = _accessorType switch
		{
			ApiClientAccessorType.User => _tokenAcquisition.GetAccessTokenForUserAsync(scope.Split(' ')),
			ApiClientAccessorType.App => _tokenAcquisition.GetAccessTokenForAppAsync(scope),
			_ => throw new InvalidOperationException()
		};
		var token = await task;

		if (!string.IsNullOrWhiteSpace(_options.SubscriptionKey))
		{
			request.Headers.Add("Ocp-Apim-Subscription-Key", _options.SubscriptionKey);
		}

		request.Headers.Authorization = new("Bearer", token);
		return await base.SendAsync(request, cancellationToken);
	}
}
