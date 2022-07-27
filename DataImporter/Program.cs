using System.Security.Cryptography;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

if (args.Any(x => x == "generate-key"))
{
    using var crypto = Aes.Create();
    var base64Key = Convert.ToBase64String(crypto.Key);
    Console.WriteLine($"Generated data file encryption key {base64Key}");
    Console.WriteLine($"use encrypt --DataEncryption:SourceDirectory \"<DIRECTORY>\" --DataEncryption:Key \"{base64Key}\" to encrypt data files with this key");
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
        .AddLogging()
        .Build();

    await host.RunAsync();

    return;
}

if (args.Any(x => x == "import"))
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.Configure<DataEncryption>(hostContext.Configuration.GetSection(nameof(DataEncryption)));
            services.AddOptions();
            services.AddNtpDbContext(hostContext.Configuration);
            services.AddDataImporter();
            services.AddHostedService<DataImporterService>();
        })
        .AddLogging()
        .Build();

    await host.RunAsync();

    return;
}

Console.WriteLine("Pass one of the following commands: generate-key encrypt import");