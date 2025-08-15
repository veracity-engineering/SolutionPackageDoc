using DNV.OAuth.Common;
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
	[Authorize(AuthenticationSchemes = Consts.Api2)]
	public class Api2Controller : ControllerBase
	{
		private readonly ILogger<Api1Controller> _logger;
		private readonly ITokenAcquisition _tokenAcquisition;
		private readonly VeracityOAuthOptions _oauthOptions;

		public Api2Controller(
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
			_logger.LogWarning("Api2");
			return this.User.Claims.ToDictionary(x => x.Type, x => x.Value);
			var token = await _tokenAcquisition.GetAccessTokenForAppAsync(_oauthOptions.DefaultAppScope);
			_logger.LogInformation("Token: {token}", token);
			var jwt = new JsonWebToken(token);
			return jwt.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
		}
	}
}
