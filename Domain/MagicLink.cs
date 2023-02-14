namespace Domain;

public class MagicLink
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(2);

    public int? EnquiryId { get; set; }

    public int? EnquiryResponseId { get; set; }

    public Enquiry? Enquiry { get; set; } = null;

    public EnquiryResponse? EnquiryResponse { get; set; } = null;
}