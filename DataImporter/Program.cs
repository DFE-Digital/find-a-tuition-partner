using Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using Infrastructure;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

if (args.Length > 0 && args.Any(x => x == "encrypt"))
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.Configure<DataEncryption>(hostContext.Configuration.GetSection(nameof(DataEncryption)));
            services.AddOptions();
            services.AddHostedService<DataEncryptionService>();
        })
        .Build();

    await host.RunAsync();
}
else
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddNtpDbContext(hostContext.Configuration);
            services.AddDataImporter();
            services.AddHostedService<DataImporterService>();
        })
        .Build();

    await host.RunAsync();
}