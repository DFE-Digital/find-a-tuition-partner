namespace Application.Common.Models.Enquiry.Build;

public record EnquiryBaseModel : SearchModel
{
    public string? BaseServiceUrl { get; set; }
    public string? ErrorMessage { get; set; }
}