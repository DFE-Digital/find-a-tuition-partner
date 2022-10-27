using CsvHelper.Configuration;

namespace SearchMetricsMapper;

public sealed class SearchMetricMap : ClassMap<SearchMetric>
{
    public SearchMetricMap()
    {
        Map(m => m.Timestamp)
            .Name("Timestamp")
            .TypeConverterOption.Format("dd/MM/yyyy HH:mm:ss");
        Map(m => m.Postcode)
            .Name("Postcode");
        Map(m => m.LadCode)
            .Name("LAD Code");
        Map(m => m.Urns)
            .Name("URNs");
        Map(m => m.ResultsCount)
            .Name("Results count");
        Map(m => m.Elapsed)
            .Name("Elapsed time in ms");
    }
}