using Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using DataImporter;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddNtpDbContext(hostContext.Configuration);
        services.AddHostedService<DataImporterService>();
    })
    .Build();

host.RunAsync();