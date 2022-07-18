using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace GiasPostcodeSearch;

public class GiasSchoolDataProvider : ISchoolDataProvider
{
    private readonly ILogger<GiasSchoolDataProvider> _logger;
    private readonly HttpClient _httpClient;

    public GiasSchoolDataProvider(ILogger<GiasSchoolDataProvider> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<SchoolDatum>> GetSchoolDataAsync(CancellationToken cancellationToken)
    {
        var dateFilename = DateTime.Today.ToString("yyyyMMdd");

        await using var response = await _httpClient.GetStreamAsync($"edubaseallstatefunded{dateFilename}.csv", cancellationToken);
        using var reader = new StreamReader(response);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<SchoolDatumMap>();
        var records = csv.GetRecords<SchoolDatum>().ToArray();

        return records;
    }

    private class SchoolDatumMap : ClassMap<SchoolDatum>
    {
        public SchoolDatumMap()
        {
            Map(m => m.Urn).Name("URN");
            Map(m => m.Name).Name("EstablishmentName");
            Map(m => m.Postcode).Name("Postcode");
            Map(m => m.LocalAuthorityCode).Name("LA (code)");
            //Map(m => m.LocalAuthorityDistrictCode).Name("EstablishmentName");
        }
    }
}