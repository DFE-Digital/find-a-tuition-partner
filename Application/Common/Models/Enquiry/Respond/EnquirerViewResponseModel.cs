namespace Application.Common.Models.Enquiry.Respond;

public record EnquirerViewResponseModel
{
    public string TuitionPartnerName { get; set; } = null!;

    public string EnquiryResponseText { get; set; } = null!;

    public string EnquirerViewAllResponsesToken { get; set; } = null!;

}