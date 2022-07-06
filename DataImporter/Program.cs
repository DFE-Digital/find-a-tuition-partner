using System.Security.Cryptography;
using Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using Infrastructure;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

if (args.Any(x => x == "generate-key"))
{
    using var crypto = Aes.Create();
    var base64Key = Convert.ToBase64String(crypto.Key);
    Console.WriteLine($"Generated data file encryption key {base64Key}");
    Console.WriteLine($"use encrypt --DataEncryption:SourceDirectory \"<DIRECTORY>\" --DataEncryption:Key {base64Key} to encrypt data files with this key");
    return;
}

if (args.Any(x => x == "encrypt"))
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