using System.ComponentModel;

namespace Domain.Search;

public class TuitionPartnerSearchRequest : SearchRequestBase
{
    public string? LocalAuthorityDistrictCode { get; set; }
    public IEnumerable<int>? SubjectIds { get; set; }
    public int? TuitionTypeId { get; set; }
    [DefaultValue(TuitionPartnerOrderBy.Random)]
    public TuitionPartnerOrderBy OrderBy { get; set; } = TuitionPartnerOrderBy.Random;
    [DefaultValue(OrderByDirection.Ascending)]
    public OrderByDirection Direction { get; set; } = OrderByDirection.Ascending;
}