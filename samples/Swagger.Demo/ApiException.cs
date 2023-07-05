using System.Net;

namespace Swagger.Demo;

public class ApiException : Exception
{
	public HttpStatusCode StatusCode { get; set; }

	public ApiException(HttpStatusCode statusCode, string? message = null, Exception? innerException = null)
		: base(message, innerException)
	{
		this.StatusCode = statusCode;
	}

	public static ApiException BadRequest(string? message = null, Exception? innerException = null)
		=> new(HttpStatusCode.BadRequest, message, innerException);

	public static ApiException InteralServerError(string? message = null, Exception? innerException = null)
		=> new(HttpStatusCode.InternalServerError, message, innerException);
}
