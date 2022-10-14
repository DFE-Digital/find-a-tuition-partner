using System.Diagnostics;
using Domain;
using Domain.Constants;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GiasPostcodeSearch;

public class GiasPostcodeSearchService : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ILogger<GiasPostcodeSearchService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HttpClient _httpClient;

    public GiasPostcodeSearchService(
        IHostApplicationLifetime host,
        ILogger<GiasPostcodeSearchService> logger,
        IServiceScopeFactory scopeFactory,
        HttpClient httpClient)
    {
        _host = host;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClient = httpClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();

        var schools = dbContext.Schools.Select(x => x).ToList();

        _logger.LogInformation("GIAS dataset loaded. {NumberOfSchools} eligible schools", schools.Count);

        var count = 0;
        var errorCount = 0;
        var totalCount = schools.Count;
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

        await Parallel.ForEachAsync(schools, options, async (school, ct) =>
        {
            var subjectsQueryString = GetSubjectsQueryString(school);
            if (subjectsQueryString == null)
            {
                _logger.LogDebug("School {SchoolName} is not valid for this service", school.EstablishmentName);
            }
            else
            {
                _logger.LogDebug("Searching for Tuition Partners covering School {SchoolName}", school.EstablishmentName);

                var requestUri = $"search-results?Data.Postcode={school.Postcode}&{subjectsQueryString}";

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

    private static string? GetSubjectsQueryString(School school)
    {
        switch (school.PhaseOfEducationId)
        {
            case (int)PhasesOfEducation.Primary:
            case (int)PhasesOfEducation.MiddleDeemedPrimary:
                return "Data.Subjects=KeyStage1-English&Data.Subjects=KeyStage2-Maths&Data.Subjects=KeyStage2-Science";
            case (int)PhasesOfEducation.Secondary:
            case (int)PhasesOfEducation.MiddleDeemedSecondary:
                return "Data.Subjects=KeyStage3-English&Data.Subjects=KeyStage3-Maths&Data.Subjects=KeyStage4-Science";
            case (int)PhasesOfEducation.AllThrough:
                return "Data.Subjects=KeyStage1-English&Data.Subjects=KeyStage1-Maths&Data.Subjects=KeyStage1-Science&Data.Subjects=KeyStage2-English&Data.Subjects=KeyStage2-Maths&Data.Subjects=KeyStage2-Science&Data.Subjects=KeyStage3-English&Data.Subjects=KeyStage3-Maths&Data.Subjects=KeyStage3-Science&Data.Subjects=KeyStage4-English&Data.Subjects=KeyStage4-Maths&Data.Subjects=KeyStage4-Science";
            default:
                return null;
        }
    }
}