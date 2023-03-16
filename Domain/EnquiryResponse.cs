namespace Domain;

public class EnquiryResponse
{
    public int Id { get; set; }

    public string KeyStageAndSubjectsText { get; set; } = null!;

    public string TuitionTypeText { get; set; } = null!;

    public string TutoringLogisticsText { get; set; } = null!;

    public string? SENDRequirementsText { get; set; }

    public string? AdditionalInformationText { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int EnquiryId { get; set; }

    public int? MagicLinkId { get; set; }

    public Enquiry Enquiry { get; set; } = null!;

    public MagicLink? MagicLink { get; set; }
}