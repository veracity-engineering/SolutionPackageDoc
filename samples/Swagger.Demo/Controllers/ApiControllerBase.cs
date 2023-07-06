using Microsoft.AspNetCore.Mvc;

namespace Swagger.Demo.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class ApiControllerBase : ControllerBase
{

}
