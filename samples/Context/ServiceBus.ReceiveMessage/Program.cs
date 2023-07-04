// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using DNV.Context.ServiceBus;
using DNV.Context.ServiceBus.Sample.Models;
using Microsoft.Extensions.Configuration;
using System.Reflection;

Console.WriteLine("This is a sample for receiving message from service bus");

var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json", false, true);
builder.AddUserSecrets<Program>(true);

var configuration = builder.Build();

var connectionString = configuration["AzureServiceBus:ConnectionString"];
var targetQueue = configuration["AzureServiceBus:TargetQueue"];


var client = new ServiceBusClient(connectionString);
var processor = client.CreateProcessor(targetQueue);

//define context
var localContextAccessor = new LocalContextAccessor<SampleContext>((args) =>
{
    return (false, null);
});

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
processor.ProcessErrorAsync += args => {

    Console.WriteLine(args.Exception.Message);
    return Task.CompletedTask;
};

await processor.StartProcessingAsync();

Console.ReadLine();