# DNV.Context.ServiceBus
The `DNV.Context.ServiceBus package` is a .Net library which provides some extension method based on `Azure.Messaging.ServiceBus` for sending and receiving messages with ambient context(`IContextAccessor<>`).

---

## Basic example
### Send Message With Context

You can self define the context payload, ex
```cs
public class SampleContext
    {
        public string Name { get; set; }
        public string SampleData { get; set; }
    }
```

Before start sending the message, get the `IContextAccessor<>` at first, and then call the extension method `SendMessageAsync<>(message, context)` with the context. It will send the context within the message. 
The correlation id will be set into `message.CorrelationId`, and the detailed context will be set into the customized key(`X-Ambient-Context-{typeof(T).Name}`) inside the message property `ApplicationProperties`.

![A sample message with context inside Service Bus Explorer](images/context/servicebusexploresample.PNG)

```cs
public class ExampleController : Controller
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
```

### Receive Message With Context

Mainly call the extension method `InitializeContext<T>(context)` of `Func<ProcessMessageEventArgs, Task>` and register it with the servicebus processor, it will help to build the context from the received message.

```cs
var client = new ServiceBusClient(connectionString);
var processor = client.CreateProcessor(targetQueue);

//define context
var localContextAccessor = new LocalContextAccessor<SampleContext>((args) =>
{
    return (false, null);
});

//define the default handler
Func<ProcessMessageEventArgs, Task> messageHandler = args =>
{
    Console.WriteLine(args.Message.Body.ToString());
    Console.WriteLine("correlationid: " + localContextAccessor.Context?.CorrelationId);
    Console.WriteLine("payload->name: "+ localContextAccessor.Context?.Payload?.Name);
    Console.WriteLine("payload->sampledata: "+localContextAccessor.Context?.Payload?.SampleData);
    
    Console.WriteLine("Thread Id: "+ Thread.CurrentThread.ManagedThreadId.ToString());
    return Task.CompletedTask;
};

//call extension method to initialize context
processor.ProcessMessageAsync += messageHandler.InitializeContext(localContextAccessor, (args) => { return (false, null); });

```
