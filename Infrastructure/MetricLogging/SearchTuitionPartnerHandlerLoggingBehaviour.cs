using System.Diagnostics;
using Domain.Search;
using MediatR;
using Microsoft.Extensions.Logging;
using static Application.Handlers.SearchTuitionPartnerHandler;

namespace Infrastructure.MetricLogging;

public class SearchTuitionPartnerHandlerLoggingBehaviour
    : IPipelineBehavior<Command, TuitionPartnerSearchResponse>
{
    private readonly ILogger _logger;

    public SearchTuitionPartnerHandlerLoggingBehaviour(
        ILogger<SearchTuitionPartnerHandlerLoggingBehaviour> logger)
        => _logger = logger;

    public async Task<TuitionPartnerSearchResponse> Handle(
        Command request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TuitionPartnerSearchResponse> next)
    {
        using var _ = _logger.BeginScope("{@Search}", request);
        using var schoolDataScope = _logger.BeginScope("{@Urn}", request.Urn);
        _logger.LogInformation("Searching for tuition partners");

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var response = await next();
        stopwatch.Stop();

        var names = response.Results.Select(x => x.Name).ToList();
        var logLevel = response.Count == 0 && string.IsNullOrWhiteSpace(request.Name) ? LogLevel.Warning : LogLevel.Information;

        using (_logger.BeginScope("{@TuitionPartners}", names))
        {
            _logger.Log(logLevel, "Found {Count} results in {Elapsed}ms",
                    response.Count, stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}