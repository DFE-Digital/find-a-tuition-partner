namespace Application.Common.Models;

public class EnquiryResponseModel
{
    public string EnquiryResponseText { get; set; } = null!;

    public int EnquiryId { get; set; }

    public string? ErrorMessage { get; set; }
}