using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Swagger.Demo.Controllers;

public class TodoController : ApiControllerBase
{
	private static readonly IList<Todo> TodoList = new List<Todo>
	{
		new() { Id = Guid.Empty, Name = "Blank" }
	};

	[HttpGet]
	public IEnumerable<Todo> GetList() => TodoList;

	[HttpGet("{id}")]
	public Todo? Get(Guid id) => TodoList.FirstOrDefault(t => t.Id == id);

	[HttpPut("{id}")]
	public Todo Put(Guid id, [FromBody] TodoWrite todo)
	{
		var item = this.Get(id) ?? throw ApiException.BadRequest();
		item.Name = todo.Name;
		item.IsComplete = todo.IsComplete;
		return item;
	}

	[HttpPost]
	public Todo Post([FromBody] TodoWrite todo)
	{
		var item = new Todo
		{
			Id = Guid.NewGuid(),
			Name = todo.Name,
			IsComplete = todo.IsComplete,
		};
		TodoList.Add(item);
		return item;
	}

	[HttpDelete("{id}")]
	public Todo Delete(Guid id)
	{
		var item = this.Get(id) ?? throw ApiException.BadRequest();
		TodoList.Remove(item);
		return item;
	}

	[HttpPatch("{id}")]
	public Todo Patch(Guid id, [FromBody] JsonPatchDocument<TodoWrite> patch)
	{
		var item = this.Get(id) ?? throw ApiException.BadRequest();
		var todo = new TodoWrite
		{
			Name = item.Name,
			IsComplete = item.IsComplete
		};
		patch.ApplyTo(todo);
		item.Name = todo.Name;
		item.IsComplete = todo.IsComplete;
		return item;
	}
}