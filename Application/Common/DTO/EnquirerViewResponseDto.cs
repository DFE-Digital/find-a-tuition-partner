namespace Application.Common.DTO;

public record EnquirerViewResponseDto
{
    public string TuitionPartnerName { get; set; } = null!;

    public DateTime? EnquiryResponseDate { get; set; }

    public string EnquirerEnquiryResponseLink { get; set; } = null!;
}