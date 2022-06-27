using Infrastructure.Extensions;
using Infrastructure.Importers;
using Microsoft.Extensions.Hosting;
using Application;
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

//var result = host.Services.GetService(typeof (INtpDbContext));



