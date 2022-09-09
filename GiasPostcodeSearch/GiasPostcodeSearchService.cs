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

    public GiasPostcodeSearchService(
        IHostApplicationLifetime host,
        ILogger<GiasPostcodeSearchService> logger,
        HttpClient httpClient,
        ISchoolDataProvider schoolDataProvider)
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

        _logger.LogInformation("GIAS dataset loaded. {NumberOfSchools} eligible schools", schoolData.Length);

        var count = 0;
        var errorCount = 0;
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

        await Parallel.ForEachAsync(schoolData, options, async (schoolDatum, ct) =>
        {
            var subjectsQueryString = GetSubjectsQueryString(schoolDatum);
            if (subjectsQueryString == null)
            {
                _logger.LogDebug("School {SchoolName} is not valid for this service", schoolDatum.Name);
            }
            else
            {
                _logger.LogDebug("Searching for Tuition Partners covering School {SchoolName}", schoolDatum?.Name);

                var requestUri = $"search-results?Data.Postcode={schoolDatum!.Postcode}&{subjectsQueryString}";

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
                    _logger.LogDebug("Response status code {StatusCode} returned after {ElapsedMilliseconds}ms", response.StatusCode, stopWatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogError("Search request {RequestUri} resulted in non success status code {StatusCode}", requestUri, response.StatusCode);
                    errorCount++;
                }

                count++;

                if (count % 500 == 0)
                {
                    averageElapsedMilliseconds = (int)(elapsedMilliseconds / (double)count);
                    searchesPerSecond = (int)((double)count / (runStopWatch.ElapsedMilliseconds / 1000));
                    _logger.LogInformation(
                        "{SearchCount} searches of {TotalCount} run in {ElapsedSeconds}s. " +
                        "Error count {ErrorCount}. {SearchesPerSecond} searches per second. " +
                        "Average response time {AverageElapsedMilliseconds}ms min {MinElapsedMilliseconds}ms ({FastestRequestUri}) max {MaxElapsedMilliseconds}ms ({slowestRequestUri})",
                        count, totalCount, runStopWatch.Elapsed / 1000, errorCount, searchesPerSecond,
                        averageElapsedMilliseconds, minElapsedMilliseconds, fastestRequestUri, maxElapsedMilliseconds, slowestRequestUri);

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
        _logger.LogInformation(
            "Run complete. {count} searches run in {runStopWatch.ElapsedMilliseconds / 1000}s. " +
            "Error count {errorCount}. {searchesPerSecond} searches per second. " +
            "Average response time {averageElapsedMilliseconds}ms min {minElapsedMilliseconds}ms " +
            "({fastestRequestUri}) max {maxElapsedMilliseconds}ms ({slowestRequestUri})",
            count, runStopWatch.ElapsedMilliseconds / 1000, errorCount, searchesPerSecond,
            averageElapsedMilliseconds, minElapsedMilliseconds, fastestRequestUri, maxElapsedMilliseconds, slowestRequestUri);

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