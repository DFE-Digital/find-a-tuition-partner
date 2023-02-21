namespace Domain;

public class TuitionPartnerEnquiry
{
    public int Id { get; set; }

    public int EnquiryId { get; set; }
    public int TuitionPartnerId { get; set; }

    public int? MagicLinkId { get; set; }
    public Enquiry Enquiry { get; set; } = null!;

    public EnquiryResponse? EnquiryResponse { get; set; }

    public MagicLink? MagicLink { get; set; }
    public TuitionPartner TuitionPartner { get; set; } = null!;
}