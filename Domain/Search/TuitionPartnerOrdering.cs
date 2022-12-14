using System.ComponentModel;
using Domain.Enums;

namespace Domain.Search;

public class TuitionPartnerOrdering
{
    [DefaultValue(TuitionPartnerOrderBy.Name)]
    public TuitionPartnerOrderBy OrderBy { get; set; } = TuitionPartnerOrderBy.Name;

    [DefaultValue(OrderByDirection.Ascending)]
    public OrderByDirection Direction { get; set; } = OrderByDirection.Ascending;

    public string[]? SeoUrlOrderBy { get; set; }

    public int? RandomSeed { get; set; }

    public static int RandomSeedGeneration(string? localAuthorityDistrictCode = null, string? postcode = null, IEnumerable<int>? subjectIds = null, int? tuitionFilterId = null)
    {
        return
            (localAuthorityDistrictCode?.Sum(x => x) ?? 0)
            + (postcode?.Sum(x => x) ?? 0)
            + (subjectIds?.Sum() ?? 0)
            + (tuitionFilterId ?? 0);
    }

}