using DNV.Context.AzureFunction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HttpAzureFunction
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly AzureFunctionAccessor<MyServiceBusContext> _serviceBusContext;

        public Function1(ILogger<Function1> logger, AzureFunctionAccessor<MyServiceBusContext> serviceBusContext)
        {
            _logger = logger;
            _serviceBusContext = serviceBusContext;
        }

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var mycontext = _serviceBusContext.Context.Payload;
            return new OkObjectResult($"Your context name is : {mycontext.Name}");
        }
    }
}
