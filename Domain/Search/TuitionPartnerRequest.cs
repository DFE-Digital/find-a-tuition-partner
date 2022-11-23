using System.ComponentModel;
using Domain.Enums;

namespace Domain.Search;

public class TuitionPartnerRequest
{
    public int[]? TuitionPartnerIds { get; set; }
    public int? LocalAuthorityDistrictId { get; set; }
    public int? Urn { get; set; }
}
