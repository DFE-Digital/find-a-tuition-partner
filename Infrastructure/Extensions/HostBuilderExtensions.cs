using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Network;

namespace Infrastructure.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
    {

        hostBuilder.UseSerilog((context, config) =>
        {
            var appLogging = configuration.GetSection("AppLogging").Get<AppLogging>();
            
            config
                .MinimumLevel.Is(appLogging.DefaultLogEventLevel)
                .MinimumLevel.Override("Microsoft", appLogging.OverrideLogEventLevel)
                .MinimumLevel.Override("System", appLogging.OverrideLogEventLevel)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console();

            if (!string.IsNullOrWhiteSpace(appLogging.TcpSinkUri))
            {
                config.WriteTo.TCPSink(appLogging.TcpSinkUri);
            }
        });

        return hostBuilder;
    }
}