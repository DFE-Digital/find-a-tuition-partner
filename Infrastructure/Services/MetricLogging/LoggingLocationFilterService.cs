using System.Diagnostics;
using Application.Common.Interfaces;
using Application.Extensions;
using Domain;
using Domain.Search;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.MetricLogging;

public class LoggingLocationFilterService : ILocationFilterService
{
    private readonly ILocationFilterService _inner;
    private readonly ILogger _logger;

    public LoggingLocationFilterService(ILocationFilterService inner, ILogger<LoggingLocationFilterService> logger)
        => (_inner, _logger) = (inner, logger);

    public async Task<LocationFilterParameters?> GetLocationFilterParametersAsync(string postcode)
    {
        var sanitisedPostcode = postcode.ToSanitisedPostcode();
        if (string.IsNullOrEmpty(sanitisedPostcode))
        {
            string errorMessage = $"Invalid postcode supplied in GetLocationFilterParametersAsync: {postcode}";
            _logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        _logger.LogInformation("Searching for postcode {Postcode}", sanitisedPostcode);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = await _inner.GetLocationFilterParametersAsync(sanitisedPostcode);
        stopwatch.Stop();

        switch (result.TryValidate())
        {
            case SuccessResult:
                _logger.LogInformation("Searching for postcode {Postcode} found {MappedPostcode} mapped to {LocalAuthorityDistrictCode} {URN} results in {Elapsed}ms",
                    sanitisedPostcode, result?.Postcode, result?.Urn, result?.LocalAuthorityDistrictCode, stopwatch.ElapsedMilliseconds);
                break;

            case ErrorResult e:
                _logger.LogInformation("Could not find postcode {Postcode} in {Elapsed}ms - {Reason}",
                    sanitisedPostcode, stopwatch.ElapsedMilliseconds, e.GetType().Name);
                break;
        }
        return result;
    }
}
