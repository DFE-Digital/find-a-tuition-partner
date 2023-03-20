namespace Application.Common.Models.Enquiry.Respond;

public record SubmittedConfirmationModel
{
    public string? SupportReferenceNumber { get; set; }
    public string? EnquirerMagicLink { get; set; }
}