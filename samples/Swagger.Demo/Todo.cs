namespace Swagger.Demo;

public class Todo
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public bool IsComplete { get; set; } = false;
}
