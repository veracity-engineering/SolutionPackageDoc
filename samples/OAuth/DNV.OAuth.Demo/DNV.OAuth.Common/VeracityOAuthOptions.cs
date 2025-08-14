using System;

namespace DNV.OAuth.Common;

public class VeracityOAuthOptions
{
	public static readonly VeracityOAuthOptions Production = new(
		VeracityEnvironment.Production,
		"a68572e3-63ce-4bc1-acdc-b64943502e9d",
		"dnvglb2cprod",
		"https://login.veracity.com",
		"83054ebf-1d7b-43f5-82ad-b2bde84d7b75"
	);
	public static readonly VeracityOAuthOptions Staging = new(
		VeracityEnvironment.Staging,
		"307530a1-6e70-4ef7-8875-daa8f5a664ec",
		"dnvglb2cstag",
		"https://loginstag.veracity.com",
		"28b7ec7b-db04-40bb-a042-b7ac5a8b36be"
	);
	public static readonly VeracityOAuthOptions Testing = new(
		VeracityEnvironment.Testing,
		"ed815121-cdfa-4097-b524-e2b23cd36eb6",
		"dnvglb2ctest",
		"https://logintest.veracity.com",
		"a4a8e726-c1cc-407c-83a0-4ce37f1ce130"
	);

	public const string LegacyInstance = "https://login.microsoftonline.com";

	public VeracityEnvironment Environment { get; protected set; }
	public string TenantId { get; protected set; }
	public string TenantName { get; protected set; }
	public string Instance { get; protected set; }
	public string DefaultResourceId { get; protected set; }
	public string UserFlow { get; protected set; } = "b2c_1a_signinwithadfsidp";
	public string Domain => $"{this.TenantName}.onmicrosoft.com";
	public string Authority => $"{this.Instance}/tfp/{this.Domain}/{this.UserFlow}/v2.0";
	public string LegacyAuthority => $"{LegacyInstance}/tfp/{this.Domain}/v2.0";
	public string DefaultUserScope => $"https://{this.Domain}/{this.DefaultResourceId}/user_impersonation";
	public string DefaultAppScope => $"https://{this.Domain}/{this.DefaultResourceId}/.default";

	public VeracityOAuthOptions(
		VeracityEnvironment environment,
		string tenantId,
		string tenantName,
		string instance,
		string defaultResourceId
	)
	{
		this.Environment = environment;
		this.TenantId = tenantId;
		this.TenantName = tenantName;
		this.Instance = instance;
		this.TenantId = tenantId;
		this.DefaultResourceId = defaultResourceId;
	}

	public string GetUserScope(string resourceId)
		=> $"https://{this.Domain}/{resourceId}/user_impersonation";

	public string GetAppScope(string resourceId)
		=> $"https://{this.Domain}/{resourceId}/.default";

	public static VeracityOAuthOptions Get(VeracityEnvironment environment)
		=> Get(environment.ToString());

	public static VeracityOAuthOptions Get(string environment)
		=> environment switch
		{
			nameof(VeracityEnvironment.Production) => VeracityOAuthOptions.Production,
			nameof(VeracityEnvironment.Staging) => VeracityOAuthOptions.Staging,
			nameof(VeracityEnvironment.Testing) => VeracityOAuthOptions.Testing,
			_ => throw new NotImplementedException()
		};
}
