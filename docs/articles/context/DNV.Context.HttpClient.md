# DNV.Context.HttpClient
The `DNV.Context.HttpClient` package is a .Net library which provides extension methods for adding context data in http client request.

---

## Basic example
### Add Context to Http Client Context Handler

First define the context payload, ex
```cs
public class Identity
{
    public string? Name { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
}
```

When application startup, call an extension method `AddHttpClientContext`. This will add a Http message handler(`HttpClientContextHandler`) to the named http client. `HttpClientContextHandler` will serialize context data to request headers before send http request.

In this example, it adds an empty `Identity` object to `HttpClientContextHandler`.

```cs
builder.Services.AddAspNetContext((context) => (true, new Identity()), null);
```

### Send Http Request With Context Data

In your controller, initialize context first, then send request with context data.

```cs
public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IContextCreator<Identity> _contextCreator;

    public HomeController(IHttpClientFactory httpClientFactory,
                            IContextCreator<Identity> contextCreator)
    {
        _httpClientFactory = httpClientFactory;
        _contextCreator = contextCreator;
    }       

    [HttpPost]
    [Route("useridentity")]
    public async Task<IActionResult> UserIdentity([FromForm] Identity userIdentity)
    {
        // Initialize context
        _contextCreator.InitializeContext(new Identity
        {
            Name = userIdentity.Name,
            Country = userIdentity.Country,
            City = userIdentity.City
        }, Guid.NewGuid().ToString());

        // Call extension method to create http client
        var client = _httpClientFactory.CreateContextClient<Identity>();
        client.BaseAddress = new Uri("https://localhost:7174");

        // Send request
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri("api/identity/user", UriKind.Relative));
        var resp = await client.SendAsync(request);
        ......
    }
}
```

In your api controller, get context from http request header, and returns to caller.

```cs
[HttpGet]
[Route("user")]
public Identity GetUser()
{            
    var headers = _httpContext?.Request.Headers[AspNetContextAccessor<Identity>.HeaderKey];
    if(headers.HasValue)
    {                
        var testContext = JsonSerializer.Deserialize<TestContext>(headers.Value.ToString(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        return testContext?.Payload!;
    }
    return new Identity();
}
```

### Display Context Data in Page

```cs
[HttpPost]
[Route("useridentity")]
public async Task<IActionResult> UserIdentity([FromForm] Identity userIdentity)
{
    ......
    
    // Receive data and returns to the view for display.
    var content = resp.Content.ReadAsStringAsync();
    var identity = JsonSerializer.Deserialize<Identity>(content.Result, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    ViewData["Name"] = identity?.Name;
    ViewData["Country"] = identity?.Country;
    ViewData["City"] = identity?.City;
    return View("Result");
}
```