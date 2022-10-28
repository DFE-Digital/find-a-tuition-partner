namespace SearchMetricsMapper;

public class SearchMetric
{
    public DateTime Timestamp { get; set; }
    public string Postcode { get; set; } = null!;
    public string LadCode { get; set; } = null!;
    public string Urns { get; set; } = string.Empty;
    public int ResultsCount { get; set; }
    public int Elapsed { get; set; }
}