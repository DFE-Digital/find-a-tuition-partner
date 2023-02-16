namespace Domain.Search;

public class TuitionPartnerResult
{
    public int Id { get; set; }
    public string SeoUrl { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Website { get; set; } = null!;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public SubjectCoverage[]? SubjectsCoverage { get; set; } = null!;
    public TuitionType[]? TuitionTypes { get; set; } = null!;
    public Price[]? Prices { get; set; } = null!;
    public bool HasLogo { get; set; }
    public string Address { get; set; } = string.Empty;
    public bool IsVatCharged { get; set; }
    public string OrganisationTypeName { get; set; } = string.Empty;
    public string RefinedDataEmptyReason { get; set; } = string.Empty;
    public bool RefinedDataEmptyReasonAppendLAName { get; set; } = false;
    public DateTime TPLastUpdatedData { get; set; }
    public DateTime ImportProcessLastUpdatedData { get; set; }
    public string ImportId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}