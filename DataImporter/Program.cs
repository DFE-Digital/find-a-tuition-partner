using Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddNtpDbContext(hostContext.Configuration);
        services.AddDataImporter();
        services.AddHostedService<DataImporterService>();
    })
    .Build();

await host.RunAsync();