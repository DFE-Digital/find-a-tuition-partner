using Microsoft.Extensions.Logging;

namespace GiasPostcodeSearch;

public class GiasSchoolDataProvider : ISchoolDataProvider
{
    private const string GiasUrl = "https://ea-edubase-api-prod.azurewebsites.net/edubase/downloads/public";

    private readonly ILogger<GiasSchoolDataProvider> _logger;
    private readonly HttpClient _httpClient;

    public GiasSchoolDataProvider(ILogger<GiasSchoolDataProvider> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<SchoolDatum>> GetSchoolDataAsync()
    {
        throw new NotImplementedException();
    }
}