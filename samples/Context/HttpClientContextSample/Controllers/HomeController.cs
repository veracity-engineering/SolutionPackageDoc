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
        private readonly IContextCreator<Identity> _contextCreator;

        public HomeController(IHttpClientFactory httpClientFactory,
                              IContextCreator<Identity> contextCreator)
        {
            _httpClientFactory = httpClientFactory;
            _contextCreator = contextCreator;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("useridentity")]
        public async Task<IActionResult> UserIdentity([FromForm] Identity userIdentity)
        {
            _contextCreator.InitializeContext(new Identity
            {
                Name = userIdentity.Name,
                Country = userIdentity.Country,
                City = userIdentity.City
            }, Guid.NewGuid().ToString());

            var client = _httpClientFactory.CreateContextClient<Identity>();
            client.BaseAddress = new Uri("https://localhost:7174");
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("api/identity/user", UriKind.Relative));
            var resp = await client.SendAsync(request);
            var content = resp.Content.ReadAsStringAsync();
            var identity = JsonSerializer.Deserialize<Identity>(content.Result);

            ViewData["Name"] = identity?.Name;
            ViewData["Country"] = identity?.Country;
            ViewData["City"] = identity?.City;

            return View("Result");
        }
    }
}