using Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using DataImporter;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddNtpDbContext(hostContext.Configuration);
        services.AddHostedService<DataImporterService>();
    })
    .Build();

host.Run();




