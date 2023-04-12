namespace Application.Common.Models.Enquiry.Manage;

public record EnquirerViewTuitionPartnerDetailsModel
{
    public string SupportReferenceNumber { get; set; } = null!;

    public string TuitionPartnerName { get; set; } = null!;

    public string TuitionPartnerPhoneNumber { get; set; } = null!;

    public string TuitionPartnerEmailAddress { get; set; } = null!;

    public string TuitionPartnerWebsite { get; set; } = null!;

    public string LocalAuthorityDistrict { get; set; } = null!;

    public string? TuitionPartnerSeoUrl { get; set; }

    public string Token { get; set; } = string.Empty;
}