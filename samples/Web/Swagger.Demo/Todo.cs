namespace Swagger.Demo;

public class Todo
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public TodoStatus Status { get; set; } = TodoStatus.Pending;
}
