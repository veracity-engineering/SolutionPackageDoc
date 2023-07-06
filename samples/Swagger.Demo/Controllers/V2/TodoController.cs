using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Swagger.Demo.Controllers.V2;

/// <summary>
/// An API Controller
/// </summary>
[ApiVersion(2)]
public class TodoController : ApiControllerBase
{
	private static readonly IList<Todo> TodoList = new List<Todo>
	{
		new() { Id = Guid.Empty, Name = "Blank" }
	};

	/// <summary>
	/// Gets a list of todo items
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	public IEnumerable<Todo> GetList() => TodoList;

	/// <summary>
	/// Throws an exception
	/// </summary>
	/// <exception cref="Exception"></exception>
	[HttpGet("notme")]
	public void NotMe() => throw new Exception();
}