namespace Domain;

public class Price
{
    public int Id { get; set; }
    public int TuitionPartnerId { get; set; }
    public TuitionPartner TuitionPartner { get; set; } = null!;
    public int TuitionTypeId { get; set; }
    public TuitionType TuitionType { get; set; } = null!;
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public int GroupSize { get; set; }
    public decimal HourlyRate { get; set; }
}