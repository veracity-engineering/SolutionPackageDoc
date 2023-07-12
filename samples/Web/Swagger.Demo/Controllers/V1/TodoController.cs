using Asp.Versioning;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Swagger.Demo.Controllers.V1;

/// <summary>
/// An API Controller
/// </summary>
[ApiVersion(1)]
public class TodoController : ApiControllerBase
{
	private static readonly IList<Todo> TodoList = new List<Todo>
	{
		new() { Id = Guid.Empty, Name = "Blank", Status = TodoStatus.Complete }
	};

	/// <summary>
	/// Gets a list of todo items
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	public IEnumerable<Todo> GetList() => TodoList;

	/// <summary>
	/// Gets a todo item by giving Id
	/// </summary>
	/// <param name="id">The Id of todo item</param>
	/// <returns></returns>
	[HttpGet("{id}")]
	public Todo? Get(Guid id) => TodoList.FirstOrDefault(t => t.Id == id);

	/// <summary>
	/// Updates a todo item
	/// </summary>
	/// <param name="id">The Id of todo item</param>
	/// <param name="todo"></param>
	/// <returns></returns>
	[HttpPut("{id}")]
	public Todo Put(Guid id, [FromBody] TodoWrite todo)
	{
		var item = this.Get(id) ?? throw ApiException.BadRequest();
		item.Name = todo.Name;
		item.Status = todo.Status;
		return item;
	}

	/// <summary>
	/// Adds a todo item
	/// </summary>
	/// <param name="todo"></param>
	/// <returns></returns>
	[HttpPost]
	public Todo Post([FromBody] TodoWrite todo)
	{
		var item = new Todo
		{
			Id = Guid.NewGuid(),
			Name = todo.Name,
			Status = todo.Status,
		};
		TodoList.Add(item);
		return item;
	}

	/// <summary>
	/// Removes a todo item
	/// </summary>
	/// <param name="id">The Id of todo item</param>
	/// <returns></returns>
	[HttpDelete("{id}")]
	public Todo Delete(Guid id)
	{
		var item = this.Get(id) ?? throw ApiException.BadRequest();
		TodoList.Remove(item);
		return item;
	}

	/// <summary>
	/// Patches a todo item
	/// </summary>
	/// <param name="id">The Id of todo item</param>
	/// <param name="patch"></param>
	/// <returns></returns>
	[HttpPatch("{id}")]
	public Todo Patch(Guid id, [FromBody] JsonPatchDocument<TodoWrite> patch)
	{
		var item = this.Get(id) ?? throw ApiException.BadRequest();
		var todo = new TodoWrite
		{
			Name = item.Name,
			Status = item.Status
		};
		patch.ApplyTo(todo);
		item.Name = todo.Name;
		item.Status = todo.Status;
		return item;
	}
}