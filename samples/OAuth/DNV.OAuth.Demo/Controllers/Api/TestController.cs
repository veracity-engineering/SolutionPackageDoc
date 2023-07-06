using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace DNV.OAuth.Demo.Controllers.Api
{
	[ApiController]
	[Route("api/[controller]")]
	public class TestController : ControllerBase
	{
        /// <summary>
        /// requires token from App1
        /// </summary>
        /// <returns></returns>
        [HttpGet("mobile")]
		[Authorize(AuthenticationSchemes = "App1")]
		public IEnumerable<KeyValuePair<string, string>> GetMobileClaims()
		{
			return this.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
		}

		/// <summary>
		/// requires token from App2
		/// </summary>
		/// <returns></returns>
		[HttpGet("janus")]
		[Authorize(AuthenticationSchemes = "App2")]
		public IEnumerable<KeyValuePair<string, string>> GetJanusClaims()
		{
			return this.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
		}
	}
}
