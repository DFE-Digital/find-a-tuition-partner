using System.ComponentModel;
using Domain.Enums;

namespace Domain.Search;

public class TuitionPartnerSearchRequest
{
    public string? Name { get; set; }
    public string? LocalAuthorityDistrictCode { get; set; }
    public string? Postcode { get; set; }
    public IEnumerable<int>? SubjectIds { get; set; }
    public int? TuitionTypeId { get; set; }
    [DefaultValue(TuitionPartnerOrderBy.Random)]
    public TuitionPartnerOrderBy OrderBy { get; set; } = TuitionPartnerOrderBy.Random;
    [DefaultValue(OrderByDirection.Ascending)]
    public OrderByDirection Direction { get; set; } = OrderByDirection.Ascending;
    public int? Urn { get; set; }
}