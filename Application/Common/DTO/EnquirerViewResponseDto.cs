namespace Application.Common.DTO;
using EnquiryResponseStatus = Domain.Enums.EnquiryResponseStatus;

public record EnquirerViewResponseDto
{
    public string TuitionPartnerName { get; set; } = null!;

    public DateTime? EnquiryResponseDate { get; set; }

    public EnquiryResponseStatus EnquiryResponseStatus { get; set; }

    public int EnquiryResponseStatusOrderBy { get; set; }
}