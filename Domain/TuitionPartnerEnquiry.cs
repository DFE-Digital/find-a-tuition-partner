namespace Domain;

public class TuitionPartnerEnquiry
{
    public int Id { get; set; }

    public int EnquiryId { get; set; }
    public int TuitionPartnerId { get; set; }

    public int MagicLinkId { get; set; }

    public int? EnquiryResponseId { get; set; }

    public DateTime ResponseCloseDate { get; set; } = DateTime.UtcNow.AddDays(7);

    public Enquiry Enquiry { get; set; } = null!;

    public EnquiryResponse? EnquiryResponse { get; set; }

    public MagicLink MagicLink { get; set; } = null!;
    public TuitionPartner TuitionPartner { get; set; } = null!;
}