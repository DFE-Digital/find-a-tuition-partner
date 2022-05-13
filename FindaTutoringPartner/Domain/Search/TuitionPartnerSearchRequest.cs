namespace Domain.Search;

public class TuitionPartnerSearchRequest : SearchRequestBase
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public TuitionPartnerOrderBy OrderBy { get; set; } = TuitionPartnerOrderBy.Name;
    public OrderByDirection Direction { get; set; } = OrderByDirection.Ascending;
}