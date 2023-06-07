namespace Domain.Search;

public class TuitionPartnersFilter
{
    public string? Name { get; set; }
    public int? LocalAuthorityDistrictId { get; set; }
    public IEnumerable<int>? SubjectIds { get; set; }
    public int? TuitionSettingId { get; set; }
    public string[]? SeoUrls { get; set; }
}