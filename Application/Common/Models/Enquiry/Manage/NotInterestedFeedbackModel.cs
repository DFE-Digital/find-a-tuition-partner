namespace Application.Common.Models.Enquiry.Manage;

public record NotInterestedFeedbackModel
{
    public string SupportReferenceNumber { get; set; } = null!;

    public string? TuitionPartnerSeoUrl { get; set; }

    public string Token { get; set; } = string.Empty;

    public string TuitionPartnerName { get; set; } = null!;

    public string? NotInterestedFeedback { get; set; }

}