using Microsoft.AspNetCore.Authentication.JwtBearer;
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
	[Authorize(AuthenticationSchemes = "OAuth2")]
	public class Api2Controller : ControllerBase
    {
        private readonly ITokenAcquisition _tokenAcquisition;

        public Api2Controller(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;

        }

        [HttpGet]
        public async Task<IEnumerable<KeyValuePair<string, string>>> Get()
        {
            var scope = "https://dnvglb2cprod.onmicrosoft.com/83054ebf-1d7b-43f5-82ad-b2bde84d7b75/.default";
            var token = await _tokenAcquisition.GetAccessTokenForAppAsync(scope);
            return this.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
        }
    }
}
