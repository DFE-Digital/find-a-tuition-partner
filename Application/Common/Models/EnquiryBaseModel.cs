using Domain.Enums;

namespace Application.Common.Models;

public class EnquiryBaseModel
{
    public int EnquiryId { get; set; }
    public string? BaseServiceUrl { get; set; }

    public string? ErrorMessage { get; set; }
}