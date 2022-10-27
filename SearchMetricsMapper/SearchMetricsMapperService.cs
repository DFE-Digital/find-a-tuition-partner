using Application.Mapping;
using CsvHelper;
using System.Globalization;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchMetricsMapper.Configuration;
using System.Reflection;
using Application;

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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IOptions<SearchMetrics>>().Value;
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
        var locationFilterService = scope.ServiceProvider.GetRequiredService<ILocationFilterService>();

        using var reader = new StreamReader(config.Source);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        csvReader.Context.RegisterClassMap<SearchMetricMap>();
        var records = csvReader.GetRecordsAsync<SearchMetric>(cancellationToken);

        var metrics = new List<SearchMetric>();

        await foreach (var record in records.WithCancellation(cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(record.LadCode))
            {
                var result = await locationFilterService.GetLocationFilterParametersAsync(record.Postcode);
                if (result?.LocalAuthorityDistrictCode == null)
                {
                    _logger.LogWarning("Postcode {Postcode} could not be mapped to a LAD code", record.Postcode);

                    continue;
                }

                record.LadCode = result.LocalAuthorityDistrictCode;
            }

            record.Urns = string.Join('|', dbContext.Schools.Where(e => e.Postcode == record.Postcode).OrderBy(e => e.Urn).Select(e => e.Urn));

            metrics.Add(record);
        }

        await using var writer = new StreamWriter(config.Output);
        await using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        {
            csvWriter.Context.RegisterClassMap<SearchMetricMap>();
            await csvWriter.WriteRecordsAsync(metrics.OrderBy(x => x.Timestamp), cancellationToken);
        }

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}