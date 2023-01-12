namespace Domain.Search;

public class TuitionPartnersFilter
{
    public string? Name { get; set; }
    public int? LocalAuthorityDistrictId { get; set; }
    public IEnumerable<int>? SubjectIds { get; set; }
    public int? TuitionTypeId { get; set; }
    public string[]? SeoUrls { get; set; }
    public bool? IsTypeOfCharity { get; set; }
}