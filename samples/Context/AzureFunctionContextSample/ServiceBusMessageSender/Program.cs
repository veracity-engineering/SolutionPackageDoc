// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using DNV.Context.ServiceBus;
using ServiceBusMessageSender;

ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

// number of messages to be sent to the queue
const int numOfMessages = 3;

// The Service Bus client types are safe to cache and use as a singleton for the lifetime
// of the application, which is best practice when messages are being published or read
// regularly.
//
// Set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
// If you use the default AmqpTcp, ensure that ports 5671 and 5672 are open.
var clientOptions = new ServiceBusClientOptions
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
//TODO: Replace the "<NAMESPACE-NAME>" and "<QUEUE-NAME>" placeholders.
client = new ServiceBusClient("use your service bus connection string");
sender = client.CreateSender("ronald-topic");

try
{
    var correlationId = Guid.NewGuid().ToString();
    var localContextAccessor = new LocalContextAccessor<MyServiceBusContext>((args) =>
    {
        return (false, null);
    });

    var myServiceBusContext = new MyServiceBusContext { Age = 22, Name = "Ronald", MyContact = new Contact() { Email = "ronald@sina.com.cn", Address = "DNV Street 16", Tel = "6589569" } };
    localContextAccessor.InitializeContext(myServiceBusContext, correlationId, null);

    var message = new ServiceBusMessage();
    message.CorrelationId = correlationId;
    message.ContentType = "application/json";
    message.Body = new BinaryData("Ronald Testing Message");
    // Use the producer client to send the batch of messages to the Service Bus queue
    await sender.SendMessageAsync(message, localContextAccessor);
    Console.WriteLine($"{message.CorrelationId} message has been published to the queue.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();

