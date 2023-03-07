namespace Domain;
public class Enquiry
{
    public int Id { get; set; }
    public string EnquiryText { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string? SupportReferenceNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TuitionPartnerEnquiry> TuitionPartnerEnquiry { get; set; } = null!;

    public ICollection<EnquiryResponse> EnquiryResponse { get; set; } = null!;

    public ICollection<MagicLink> MagicLinks { get; set; } = null!;
}