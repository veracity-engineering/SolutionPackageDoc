using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.OAuth.Demo.Controllers.Api
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = Consts.Api1 + "," + Consts.Api2)]
	public class Api1Controller : ControllerBase
	{
		private readonly ILogger<Api1Controller> _logger;

		public Api1Controller(ILogger<Api1Controller> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public async Task<IEnumerable<KeyValuePair<string, string>>> Get()
		{
			_logger.LogWarning("Api1");
			return this.User.Claims.ToDictionary(x => x.Type, x => x.Value);
		}
	}
}
