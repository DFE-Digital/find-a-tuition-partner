namespace Domain.Search;

public class TuitionPartnerSearchRequest : SearchRequestBase
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}