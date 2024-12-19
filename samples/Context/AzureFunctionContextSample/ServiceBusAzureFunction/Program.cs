using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DNV.Context.AzureFunction;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(workerApplication =>
    {
        workerApplication.UseAzureFunctionContext<MyServiceBusContext>();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddAzureFunctionContext<MyServiceBusContext>(null, null);
    })
    .Build();

host.Run();
