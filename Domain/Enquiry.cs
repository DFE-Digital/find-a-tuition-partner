namespace Domain;
public class Enquiry
{
    public int Id { get; set; }
    public string EnquiryText { get; set; } = null!;
    public string Email { get; set; } = null!;

    public List<TuitionPartnerEnquirySeoUrl> TuitionPartnerEnquirySeoUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}