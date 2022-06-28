namespace Domain;

public class Subject
{
    public int Id { get; set; }
    public int KeyStageId { get; set; }
    public KeyStage KeyStage { get; set; } = null!;
    public string Name { get; set; } = null!;
}