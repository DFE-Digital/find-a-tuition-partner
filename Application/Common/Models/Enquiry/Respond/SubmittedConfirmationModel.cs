namespace Application.Common.Models.Enquiry.Respond;

public record SubmittedConfirmationModel
{
    public string? SupportReferenceNumber { get; set; }
    public string? EnquirerMagicLink { get; set; }
    public string? LocalAuthorityDistrictName { get; set; }
    public string TuitionPartnerName { get; set; } = string.Empty;

    public string ErrorStatus { get; set; } = string.Empty;
    public bool IsValid { get; set; } = true;
}