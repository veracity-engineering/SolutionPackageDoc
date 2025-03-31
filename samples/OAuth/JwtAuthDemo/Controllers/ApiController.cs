using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
	[Authorize(AuthenticationSchemes = "Api1,Api2,Api3,Api4")]
	[HttpGet]
	public string Get()
	{
		return "Hello, World!";
	}
}
