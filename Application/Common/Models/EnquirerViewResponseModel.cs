namespace Application.Common.Models;

public class EnquirerViewResponseModel
{
    public string TuitionPartnerName { get; set; } = null!;

    public string? Status { get; set; } = null!;

    public string EnquiryResponse { get; set; } = string.Empty;
}