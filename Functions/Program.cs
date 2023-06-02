using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(builder =>
    {
        builder.AddHttpClient("PollEmailProcessing");
    })
    .Build();

await host.RunAsync();
