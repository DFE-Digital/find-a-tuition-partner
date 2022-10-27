using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchMetricsMapper.Configuration;

namespace SearchMetricsMapper;

public class SearchMetricsMapperService : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ILogger<SearchMetricsMapperService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public SearchMetricsMapperService(IHostApplicationLifetime host, ILogger<SearchMetricsMapperService> logger, IServiceScopeFactory scopeFactory)
    {
        _host = host;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IOptions<SearchMetrics>>().Value;
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();

        _host.StopApplication();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}