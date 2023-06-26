namespace Application.Common.Models.Enquiry.Manage;

public record NotInterestedFeedbackModel
{
    public string SupportReferenceNumber { get; set; } = null!;

    public string? TuitionPartnerSeoUrl { get; set; }

    public string Token { get; set; } = string.Empty;

    public string TuitionPartnerName { get; set; } = null!;

    public int? EnquirerNotInterestedReasonId { get; set; }

    public string? EnquirerNotInterestedReasonAdditionalInfo { get; set; }

    public bool MustCollectAdditionalInfo { get; set; }

    public List<EnquirerNotInterestedReasonModel> EnquirerNotInterestedReasonModels { get; set; } = null!;

}