namespace Domain;

public class KeyStageSubjectEnquiry
{
    public int Id { get; set; }

    public int EnquiryId { get; set; }

    public int KeyStageId { get; set; }

    public int SubjectId { get; set; }

    public Enquiry Enquiry { get; set; } = null!;

    public KeyStage KeyStage { get; set; } = null!;

    public Subject Subject { get; set; } = null!;
}