using DNV.Context.AspNet;
using HttpClientContextSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HttpClientContextSample.Controllers.Api
{
    [ApiController]
    [Route("api/identity")]
    public class IdentityController : Controller
    {
        private readonly HttpContext? _httpContext;

        public IdentityController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        [HttpGet]
        [Route("user")]
        public Identity GetUser()
        {            
            var headers = _httpContext?.Request.Headers[AspNetContextAccessor<Identity>.HeaderKey];
            if(headers.HasValue)
            {                
                var testContext = JsonSerializer.Deserialize<TestContext>(headers.Value.ToString(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
                return testContext?.Payload!;
            }
            return new Identity();
        }
    }
}
