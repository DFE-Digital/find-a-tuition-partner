namespace Domain;

public class ReferenceDatum
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public ReferenceDataType Type { get; set; }
    public bool IsInactive { get; set; }
}