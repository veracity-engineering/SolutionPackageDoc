using DNV.OAuth.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.OAuth.Demo.Controllers.Api
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class Api1Controller : ControllerBase
	{
		private readonly ILogger<Api1Controller> _logger;
		private readonly ITokenAcquisition _tokenAcquisition;
		private readonly VeracityOAuthOptions _oauthOptions;

		public Api1Controller(
			ILogger<Api1Controller> logger,
			ITokenAcquisition tokenAcquisition, 
			VeracityOAuthOptions oauthOptions
		)
		{
			_logger = logger;
			_tokenAcquisition = tokenAcquisition;
			_oauthOptions = oauthOptions;
		}

		[HttpGet]
		public async Task<IEnumerable<KeyValuePair<string, string>>> Get()
		{
			var scope = _oauthOptions.DefaultAppScope;
			var token = await _tokenAcquisition.GetAccessTokenForAppAsync(scope);
			_logger.LogInformation("Token: {token}", token);
			var jwt = new JsonWebToken(token);
			return jwt.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
		}
	}
}
