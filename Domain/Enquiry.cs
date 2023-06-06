namespace Domain;
public class Enquiry
{
    public int Id { get; set; }
    public string TutoringLogistics { get; set; } = null!;
    public string? SENDRequirements { get; set; }
    public string? AdditionalInformation { get; set; }
    public string Email { get; set; } = null!;
    public string SupportReferenceNumber { get; set; } = null!;

    public string PostCode { get; set; } = null!;

    public string LocalAuthorityDistrict { get; set; } = null!;

    public int? SchoolId { get; set; }

    public int MagicLinkId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public School School { get; set; } = null!;

    public ICollection<TuitionSetting>? TuitionSettings { get; set; } = null!;

    public MagicLink MagicLink { get; set; } = null!;

    public ICollection<TuitionPartnerEnquiry> TuitionPartnerEnquiry { get; set; } = null!;

    public ICollection<KeyStageSubjectEnquiry> KeyStageSubjectEnquiry { get; set; } = null!;
}