namespace Swagger.Demo;

public class TodoWrite
{
	public string? Name { get; set; }
	public TodoStatus Status { get; set; } = TodoStatus.Pending;
}
