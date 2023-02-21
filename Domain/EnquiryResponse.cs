namespace Domain;

public class EnquiryResponse
{
    public int Id { get; set; }

    public string EnquiryResponseText { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int EnquiryId { get; set; }

    public int? MagicLinkId { get; set; }

    public Enquiry Enquiry { get; set; } = null!;

    public MagicLink? MagicLink { get; set; }
}