namespace Application.Common.Models.Enquiry.Respond;

public record ResponseConfirmationModel
{
    public string? SupportReferenceNumber { get; set; }
    public string? EnquirerMagicLink { get; set; }
    public string TuitionPartnerName { get; set; } = string.Empty;
}