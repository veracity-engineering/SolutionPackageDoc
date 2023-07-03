using Azure.Messaging.ServiceBus;
using DNV.Context.Abstractions;
using DNV.Context.ServiceBus.Sample.SendMessage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace DNV.Context.ServiceBus.Sample.SendMessage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContextAccessor<SampleContext> _contextAccessor;
        private readonly ServiceBusConfig _serviceBusConfig;

        public HomeController(ILogger<HomeController> logger, IContextAccessor<SampleContext> contextAccessor, IOptionsSnapshot<ServiceBusConfig> serviceBusOptions)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _serviceBusConfig = serviceBusOptions.Value;
        }

        public IActionResult Index()
        {
            return View(_contextAccessor);
        }

        [Route("/sendmessage"), HttpPost]        
        public async Task<IActionResult> SendMessage()
        {
            var client = new ServiceBusClient(_serviceBusConfig.ConnectionString);
            var sender = client.CreateSender(_serviceBusConfig.TargetQueue);

            //send message with context
            await sender.SendMessageAsync(new ServiceBusMessage("test"), _contextAccessor);

            return Ok();
        }

    }
}