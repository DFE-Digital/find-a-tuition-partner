using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Network;

namespace Infrastructure.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddLogging(this IHostBuilder hostBuilder, LogEventLevel? overrideLogEventLevel = null)
    {
        hostBuilder.UseSerilog((context, config) =>
        {
            var appLogging = context.Configuration.GetSection("AppLogging").Get<AppLogging>() ?? new AppLogging();

            overrideLogEventLevel ??= appLogging.OverrideLogEventLevel;

            config
                .MinimumLevel.Is(appLogging.DefaultLogEventLevel)
                .MinimumLevel.Override("Microsoft", overrideLogEventLevel.Value)
                .MinimumLevel.Override("System", overrideLogEventLevel.Value)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console();
        });

        return hostBuilder;
    }
}