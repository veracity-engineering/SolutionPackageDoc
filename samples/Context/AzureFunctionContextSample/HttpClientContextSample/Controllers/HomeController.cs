using DNV.Context.Abstractions;
using DNV.Context.HttpClient;
using HttpClientContextSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HttpClientContextSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IContextCreator<MyServiceBusContext> _contextCreator;

        public HomeController(IHttpClientFactory httpClientFactory,
                              IContextCreator<MyServiceBusContext> contextCreator)
        {
            _httpClientFactory = httpClientFactory;
            _contextCreator = contextCreator;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("TestContext")]
        public async Task<IActionResult> TestContext()
        {
            _contextCreator.InitializeContext(new MyServiceBusContext
            { Age = 22, Name = "Ronald", MyContact = new Contact() { Email = "ronald@sina.com.cn", Address = "DNV Street 16", Tel = "6589569" } }, Guid.NewGuid().ToString());

            var client = _httpClientFactory.CreateContextClient<MyServiceBusContext>();
            client.BaseAddress = new Uri("http://localhost:7013");
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("api/Function1", UriKind.Relative));
            var resp = await client.SendAsync(request);
            var content = await resp.Content.ReadAsStringAsync();
            ViewData["Message"] = content;
            return View("Result");
        }
    }
}