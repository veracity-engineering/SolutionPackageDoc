using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.OAuth.Demo.Controllers.Api
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Policy = "Api")]
	public class Api1Controller : ControllerBase
	{
		private readonly ITokenAcquisition _tokenAcquisition;

		public Api1Controller(ITokenAcquisition tokenAcquisition)
		{
			_tokenAcquisition = tokenAcquisition;
		}

		[HttpGet]
		[Authorize]
		public async Task<IEnumerable<KeyValuePair<string, string>>> Get()
		{
			var scope = "https://dnvglb2cprod.onmicrosoft.com/af785728-e2d5-4b58-a263-d1a11c5e21f0/.default";
			var token = await _tokenAcquisition.GetAccessTokenForAppAsync(scope);
			return this.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
		}
	}
}
