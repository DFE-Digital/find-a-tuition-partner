using Infrastructure.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

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

            var appInsightInstrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];

            if (!string.IsNullOrEmpty(appInsightInstrumentationKey))
            {
                var telemetryConfigs = TelemetryConfiguration.CreateDefault();
                telemetryConfigs.InstrumentationKey = appInsightInstrumentationKey;
                var telemetryClient = new TelemetryClient(telemetryConfigs);

                config.WriteTo.ApplicationInsights(telemetryClient, TelemetryConverter.Traces);
            }

        });

        return hostBuilder;
    }
}