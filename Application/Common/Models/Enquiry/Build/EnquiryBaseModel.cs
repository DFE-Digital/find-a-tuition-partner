namespace Application.Common.Models.Enquiry.Build;

public class EnquiryBaseModel
{
    public int EnquiryId { get; set; }
    public string? BaseServiceUrl { get; set; }

    public string? ErrorMessage { get; set; }
}