using DNV.Veracity.Services.Api.Models;
using DNV.Veracity.Services.Api.My.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNV.OAuth.Demo.Controllers.Api
{
	[ApiController]
	[Route("api/[controller]")]
	public class TestController : ControllerBase
	{
		private readonly IMyProfile _myProfile;

		public TestController(IMyProfile myProfile)
		{
			_myProfile = myProfile ?? throw new ArgumentNullException(nameof(myProfile));
		}

		/// <summary>
		/// requires token from Veracity
		/// </summary>
		/// <returns></returns>
		[HttpGet("mobile")]
		[Authorize(AuthenticationSchemes = "Veracity")]
		public Task<Profile> GetMyProfile()
		{
			return _myProfile.Get();
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
