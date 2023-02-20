namespace Domain;

public class TuitionPartnerEnquiry
{
    public int Id { get; set; }

    public int EnquiryId { get; set; }
    public int TuitionPartnerId { get; set; }

    public Enquiry Enquiry { get; set; } = null!;

    public TuitionPartner TuitionPartner { get; set; } = null!;
}