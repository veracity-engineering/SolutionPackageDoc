using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DNV.Context.AzureFunction;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ServiceBusAzureFunction
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly AzureFunctionAccessor<MyServiceBusContext> _serviceBusContext;
        public Function1(ILogger<Function1> logger, AzureFunctionAccessor<MyServiceBusContext> myServiceBusContext)
        {
            _logger = logger;
            _serviceBusContext = myServiceBusContext;
        }

        [Function(nameof(Function1))]
        public async Task Run(
            [ServiceBusTrigger("ronald-topic", "ronald-sub", Connection = "MyServiceBus")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
            var mycontext = _serviceBusContext!.Context!.Payload;
            _logger.LogInformation($"context name: {mycontext!.Name}");
            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
