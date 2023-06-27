namespace Application.Common.Models.Enquiry.Manage;
using EnquiryResponseStatus = Domain.Enums.EnquiryResponseStatus;

public record EnquirerViewResponseModel : EnquiryResponseBaseModel
{
    public EnquiryResponseStatus EnquiryResponseStatus { get; set; }
}