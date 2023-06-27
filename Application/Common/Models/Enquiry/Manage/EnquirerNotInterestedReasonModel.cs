namespace Application.Common.Models.Enquiry.Manage;

public record EnquirerNotInterestedReasonModel
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public bool CollectAdditionalInfoIfSelected { get; set; }

}