namespace Application.Common.Models;

public class EnquirerViewAllResponsesModel
{
    public string EnquiryText { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public List<EnquirerViewResponseModel> EnquirerViewResponses { get; set; } = null!;
}