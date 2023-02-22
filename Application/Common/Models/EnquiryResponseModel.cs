namespace Application.Common.Models;

public class EnquiryResponseModel : EnquiryBaseModel
{
    public int TuitionPartnerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string EnquiryText { get; set; } = string.Empty;
    public string EnquiryResponseText { get; set; } = null!;
}