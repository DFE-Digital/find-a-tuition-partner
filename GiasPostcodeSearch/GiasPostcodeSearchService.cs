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
        var schoolData = (await _schoolDataProvider.GetSchoolDataAsync(cancellationToken)).Where(x =>
            new[]
            {
                PhaseOfEducation.Primary, PhaseOfEducation.MiddleDeemedPrimary, PhaseOfEducation.Secondary,
                PhaseOfEducation.MiddleDeemedSecondary, PhaseOfEducation.AllThrough
            }.Contains(x.PhaseOfEducation)).ToArray();

        _logger.LogInformation($"GIAS dataset loaded. {schoolData.Length} eligible schools");

        var count = 0;
        var totalCount = schoolData.Length;
        var elapsedMilliseconds = 0L;
        var minElapsedMilliseconds = long.MaxValue;
        var maxElapsedMilliseconds = 0L;
        var fastestRequestUri = "";
        var slowestRequestUri = "";
        long averageElapsedMilliseconds;
        decimal searchesPerSecond;

        var runStopWatch = new Stopwatch();
        runStopWatch.Start();

        var options = new ParallelOptions
        {
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(schoolData, options, async (schoolDatum, ct) => {
            var subjectsQueryString = GetSubjectsQueryString(schoolDatum);
            if (subjectsQueryString == null)
            {
                _logger.LogDebug($"School {schoolDatum} is not valid for this service");
            }
            else
            {
                _logger.LogDebug($"Searching for Tuition Partners covering School {schoolDatum}");
                
                var requestUri = $"search-results?Data.Postcode={schoolDatum.Postcode}&{subjectsQueryString}";
                
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var response = await _httpClient.GetAsync(requestUri, ct);
                stopWatch.Stop();
                elapsedMilliseconds += stopWatch.ElapsedMilliseconds;

                if (count > 100)
                {
                    // Only update fastest and slowest response after the first 100 requests to give service time to warm up
                    if (stopWatch.ElapsedMilliseconds < minElapsedMilliseconds)
                    {
                        minElapsedMilliseconds = stopWatch.ElapsedMilliseconds;
                        fastestRequestUri = requestUri;
                    }

                    if (stopWatch.ElapsedMilliseconds > maxElapsedMilliseconds)
                    {
                        maxElapsedMilliseconds = stopWatch.ElapsedMilliseconds;
                        slowestRequestUri = requestUri;
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug($"Response status code {response.StatusCode} returned after {stopWatch.ElapsedMilliseconds}ms");
                }
                else
                {
                    _logger.LogError($"Search request {requestUri} resulted in non success status code {response.StatusCode}");
                }

                count++;

                if (count % 500 == 0)
                {
                    averageElapsedMilliseconds = (int)(elapsedMilliseconds / (double)count);
                    searchesPerSecond = (int)((double)count / (runStopWatch.ElapsedMilliseconds / 1000));
                    _logger.LogInformation($"{count} searches of {totalCount} run in {runStopWatch.ElapsedMilliseconds / 1000}s. {searchesPerSecond} searches per second. Average response time {averageElapsedMilliseconds}ms min {minElapsedMilliseconds}ms ({fastestRequestUri}) max {maxElapsedMilliseconds}ms ({slowestRequestUri})");
                    
                    // Reset metrics for next run
                    minElapsedMilliseconds = long.MaxValue;
                    maxElapsedMilliseconds = 0L;
                    fastestRequestUri = "";
                    slowestRequestUri = "";
                }
            }
        });

        runStopWatch.Stop();
        averageElapsedMilliseconds = (int)(elapsedMilliseconds / (double)count);
        searchesPerSecond = (int)((double)count / runStopWatch.ElapsedMilliseconds);
        _logger.LogInformation($"Run complete. {count} searches run in {runStopWatch.ElapsedMilliseconds/1000}s. {searchesPerSecond} searches per second. Average response time {averageElapsedMilliseconds}ms min {minElapsedMilliseconds}ms ({fastestRequestUri}) max {maxElapsedMilliseconds}ms ({slowestRequestUri})");

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private string? GetSubjectsQueryString(SchoolDatum schoolDatum)
    {
        switch (schoolDatum.PhaseOfEducation)
        {
            case PhaseOfEducation.Primary:
            case PhaseOfEducation.MiddleDeemedPrimary:
                return "Data.Subjects=KeyStage1-English&Data.Subjects=KeyStage2-Maths&Data.Subjects=KeyStage2-Science";
            case PhaseOfEducation.Secondary:
            case PhaseOfEducation.MiddleDeemedSecondary:
                return "Data.Subjects=KeyStage3-English&Data.Subjects=KeyStage3-Maths&Data.Subjects=KeyStage4-Science";
            case PhaseOfEducation.AllThrough:
                return "Data.Subjects=KeyStage1-English&Data.Subjects=KeyStage1-Maths&Data.Subjects=KeyStage1-Science&Data.Subjects=KeyStage2-English&Data.Subjects=KeyStage2-Maths&Data.Subjects=KeyStage2-Science&Data.Subjects=KeyStage3-English&Data.Subjects=KeyStage3-Maths&Data.Subjects=KeyStage3-Science&Data.Subjects=KeyStage4-English&Data.Subjects=KeyStage4-Maths&Data.Subjects=KeyStage4-Science";
            default: return null;
        }
    }
}