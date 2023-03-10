namespace Application.Common.DTO;

public record MagicLinkDto
{
    public string Token { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }

    public int? EnquiryId { get; set; }

    public int? MagicLinkTypeId { get; set; }
}