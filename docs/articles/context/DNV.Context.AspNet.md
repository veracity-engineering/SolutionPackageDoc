# DNV.Context.AspNet
The `DNV.Context.AspNet` package is a .Net library which provides some extension methods, middleware for create or parse 
ambient context(`IContextAccessor<>`) in Asp .Net web applications.

---

## Basic example
### Get Data From Context

First define the context payload, ex
```cs
public record Identity
{
    public string? Name { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
}
```

When application startup, call extension method `AddAspNetContext`, pass in custom context payload creator. In this example, it is like below:

```cs
builder.Services.AddAspNetContext((context) => (true, new Identity
{
    Name = "John Doe",
    Country = "Norway",
    City = "Oslo"
}));
```

Register a middleware when startup. This will initialize the context using custom context payload creator which is passed in `AddAspNetContext`.

```cs
app.UseAspNetContext<Identity>();
```

Then you can get the context data in a controller action.

```cs
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AspNetContextAccessor<Identity> _accessor;

    public HomeController(ILogger<HomeController> logger,
                            AspNetContextAccessor<Identity> accessor)
    {
        _accessor = accessor;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var identity = _accessor.Context?.Payload;
        ViewData["Name"] = identity?.Name;
        ViewData["Country"] = identity?.Country;
        ViewData["City"] = identity?.City;
        return View();
    }            
}
```

The display result will be:

![A sample with data inside AspNetContext](../images/context/aspnetcontextsample.PNG)