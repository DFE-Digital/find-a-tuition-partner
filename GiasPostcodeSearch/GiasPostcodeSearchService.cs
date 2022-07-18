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
        var schoolData = await _schoolDataProvider.GetSchoolDataAsync(cancellationToken);

        var count = 0;
        var totalElapsedMilliseconds = 0L;
        var minElapsedMilliseconds = long.MaxValue;
        var maxElapsedMilliseconds = 0L;
        string fastestRequestUri = "";
        string slowestRequestUri = "";

        var runStopWatch = new Stopwatch();
        runStopWatch.Start();

        foreach (var schoolDatum in schoolData)
        {
            _logger.LogDebug($"Searching for Tuition Partners covering School {schoolDatum}");

            var subjectsQueryString = GetSubjectsQueryString(schoolDatum);
            if (subjectsQueryString == null)
            {
                _logger.LogInformation($"School {schoolDatum} is not valid for this service");
            }

            var requestUri = $"search-results?Postcode={schoolDatum.Postcode}&{subjectsQueryString}";
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            stopWatch.Stop();
            totalElapsedMilliseconds += stopWatch.ElapsedMilliseconds;
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

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Response status code {response.StatusCode} returned after {stopWatch.ElapsedMilliseconds}ms");
            }
            else
            {
                _logger.LogError($"Search request {requestUri} resulted in non success status code {response.StatusCode}");
            }

            count++;

            if (count == 10) break;
        }

        runStopWatch.Stop();
        var averageElapsedMilliseconds = (int)(runStopWatch.ElapsedMilliseconds / (double)count);
        _logger.LogInformation($"{count} searches run in {runStopWatch.ElapsedMilliseconds/1000}s. Average response time {averageElapsedMilliseconds}ms min {minElapsedMilliseconds}ms ({fastestRequestUri}) max {maxElapsedMilliseconds}ms ({slowestRequestUri})");

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