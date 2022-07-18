using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GiasPostcodeSearch;

public class GiasPostcodeSearchService : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ILogger<GiasPostcodeSearchService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ISchoolDataProvider _schoolDataProvider;

    public GiasPostcodeSearchService(IHostApplicationLifetime host, ILogger<GiasPostcodeSearchService> logger, HttpClient httpClient, ISchoolDataProvider schoolDataProvider)
    {
        _host = host;
        _logger = logger;
        _httpClient = httpClient;
        _schoolDataProvider = schoolDataProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var schoolData = await _schoolDataProvider.GetSchoolDataAsync();

        var count = 0;
        var totalElapsedMilliseconds = 0L;
        var minElapsedMilliseconds = long.MaxValue;
        var maxElapsedMilliseconds = 0L;

        foreach (var schoolDatum in schoolData)
        {
            _logger.LogDebug($"Searching for Tuition Partners covering {schoolDatum.Name}'s postcode {schoolDatum.Postcode}");

            var requestUri = $"search-results?Postcode={schoolDatum.Postcode}";
            
            var stopWatch = new Stopwatch();
            stopWatch.Stop();
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            stopWatch.Start();
            totalElapsedMilliseconds += stopWatch.ElapsedMilliseconds;
            minElapsedMilliseconds = Math.Min(minElapsedMilliseconds, stopWatch.ElapsedMilliseconds);
            maxElapsedMilliseconds = Math.Max(minElapsedMilliseconds, stopWatch.ElapsedMilliseconds);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Response status code {response.StatusCode} returned after {stopWatch.ElapsedMilliseconds}ms");
            }
            else
            {
                _logger.LogError($"Search request {requestUri} resulted in non success status code {response.StatusCode}");
            }

            count++;
        }

        var averageElapsedMilliseconds = (int)(totalElapsedMilliseconds / (double)count);
        _logger.LogInformation($"{count} searches run. Average response time {averageElapsedMilliseconds} min {minElapsedMilliseconds} max {maxElapsedMilliseconds}");

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}