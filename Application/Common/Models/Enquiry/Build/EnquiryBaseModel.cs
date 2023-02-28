namespace Application.Common.Models.Enquiry.Build;

public record EnquiryBaseModel : SearchModel
{
    public int EnquiryId { get; set; }
    public string? BaseServiceUrl { get; set; }

    public string? ErrorMessage { get; set; }
}