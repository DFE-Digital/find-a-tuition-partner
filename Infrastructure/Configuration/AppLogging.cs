using Serilog.Events;

namespace Infrastructure.Configuration;

public class AppLogging
{
    public LogEventLevel DefaultLogEventLevel { get; set; } = LogEventLevel.Debug;
    public LogEventLevel OverrideLogEventLevel { get; set; } = LogEventLevel.Information;
    public string TcpSinkUri { get; set; } = string.Empty;
}