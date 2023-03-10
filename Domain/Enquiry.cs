namespace Domain;
public class Enquiry
{
    public int Id { get; set; }
    public string TutoringLogistics { get; set; } = null!;
    public string? SendRequirements { get; set; }
    public string? AdditionalInformation { get; set; }
    public string Email { get; set; } = null!;
    public string SupportReferenceNumber { get; set; } = null!;

    public string PostCode { get; set; } = null!;

    public string LocalAuthorityDistrict { get; set; } = null!;

    public int? TuitionTypeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TuitionType TuitionType { get; set; } = null!;

    public ICollection<TuitionPartnerEnquiry> TuitionPartnerEnquiry { get; set; } = null!;

    public ICollection<EnquiryResponse> EnquiryResponse { get; set; } = null!;

    public ICollection<MagicLink> MagicLinks { get; set; } = null!;

    public ICollection<KeyStageSubjectEnquiry> KeyStageSubjectEnquiry { get; set; } = null!;
}