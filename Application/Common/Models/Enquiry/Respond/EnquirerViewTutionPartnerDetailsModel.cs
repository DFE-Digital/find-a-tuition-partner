namespace Application.Common.Models.Enquiry.Respond;

public record EnquirerViewTutionPartnerDetailsModel
{
    public string SupportReferenceNumber { get; set; } = null!;

    public string EnquirerViewAllResponsesToken { get; set; } = null!;

    public string EnquirerViewResponseToken { get; set; } = null!;

    public string TuitionPartnerName { get; set; } = null!;

    public string TuitionPartnerPhoneNumber { get; set; } = null!;

    public string TuitionPartnerEmailAddress { get; set; } = null!;


}