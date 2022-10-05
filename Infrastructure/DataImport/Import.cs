using System.Reflection;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;

namespace Infrastructure.DataImport;

public static class Import
{
    public static async Task<bool> RunImport(string[] args)
    {
        if (args.Any(x => x == "import"))
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    config.AddUserSecrets(Assembly.GetExecutingAssembly());
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddNtpDbContext(hostContext.Configuration);
                    services.AddDataImporter(hostContext.Configuration);
                })
                .AddLogging(LogEventLevel.Warning)
                .Build();

            await host.RunAsync();

            return true;
        }
        else
            return false;
    }
}