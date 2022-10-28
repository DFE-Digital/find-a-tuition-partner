using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SearchMetricsMapper;
using SearchMetricsMapper.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<SearchMetrics>(hostContext.Configuration.GetSection(nameof(SearchMetrics)));
        services.AddNtpDbContext(hostContext.Configuration);
        services.AddLocationFilterService();
        services.AddRepositories();
        services.AddHostedService<SearchMetricsMapperService>();
    })
    .AddLogging()
    .Build();

await host.RunAsync();