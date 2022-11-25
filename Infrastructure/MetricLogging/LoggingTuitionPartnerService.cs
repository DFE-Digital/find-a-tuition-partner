using System.Diagnostics;
using Application;
using Domain.Enums;
using Domain.Search;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MetricLogging;

public class LoggingTuitionPartnerService : ITuitionPartnerService
{
    private readonly ITuitionPartnerService _inner;
    private readonly ILogger _logger;

    public LoggingTuitionPartnerService(ITuitionPartnerService inner, ILogger<LoggingTuitionPartnerService> logger)
        => (_inner, _logger) = (inner, logger);

    public async Task<int[]?> GetTuitionPartnersFilteredAsync(TuitionPartnersFilter filter, CancellationToken cancellationToken)
    {
        using var _ = _logger.BeginScope("{@Filter}", filter);
        _logger.LogInformation("Filtering tuition partners");

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = await _inner.GetTuitionPartnersFilteredAsync(filter, cancellationToken);
        stopwatch.Stop();

        var resultCount = result?.Length;

        var logLevel = (resultCount == 0 && string.IsNullOrWhiteSpace(filter.Name)) ? LogLevel.Warning : LogLevel.Information;

        _logger.Log(logLevel, "Found {Count} TPs in {Elapsed}ms",
                   resultCount, stopwatch.ElapsedMilliseconds);

        return result;
    }

    public async Task<IEnumerable<TuitionPartnerResult>> GetTuitionPartnersAsync(TuitionPartnerRequest request, CancellationToken cancellationToken)
    {
        using var _ = _logger.BeginScope("{@Request}", request);
        using var schoolDataScope = _logger.BeginScope("{@Urn}", request.Urn);
        _logger.LogInformation("Get tuition partners");

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = await _inner.GetTuitionPartnersAsync(request, cancellationToken);
        stopwatch.Stop();

        var names = result.Select(x => x.Name).ToList();
        var logLevel = ((request.TuitionPartnerIds == null && !result.Any()) || (request.TuitionPartnerIds?.Length != result.Count())) ?
            LogLevel.Error : LogLevel.Information;

        using (_logger.BeginScope("{@TuitionPartners}", names))
        {
            _logger.Log(logLevel, "Found {Count} TP results in {Elapsed}ms",
                    result.Count(), stopwatch.ElapsedMilliseconds);
        }

        return result;
    }

    public IEnumerable<TuitionPartnerResult> OrderTuitionPartners(IEnumerable<TuitionPartnerResult> results, TuitionPartnerOrdering ordering)
    {
        _logger.LogInformation("Order tuition partners");

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = _inner.OrderTuitionPartners(results, ordering);
        stopwatch.Stop();

        _logger.LogInformation("Ordered {Count} TP results by {OrderBy} in {Elapsed}ms",
                    result.Count(), ordering.OrderBy.ToString(), stopwatch.ElapsedMilliseconds);

        return result;
    }
}
