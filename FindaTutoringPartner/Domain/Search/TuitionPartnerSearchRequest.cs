using System.ComponentModel;

namespace Domain.Search;

public class TuitionPartnerSearchRequest : SearchRequestBase
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    [DefaultValue(TuitionPartnerOrderBy.Name)]
    public TuitionPartnerOrderBy OrderBy { get; set; } = TuitionPartnerOrderBy.Name;
    [DefaultValue(OrderByDirection.Ascending)]
    public OrderByDirection Direction { get; set; } = OrderByDirection.Ascending;
}