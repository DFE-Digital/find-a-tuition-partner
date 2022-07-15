namespace Domain;

public class Subject
{
    public int Id { get; set; }
    public string SeoUrl { get; set; } = null!;
    public int KeyStageId { get; set; }
    public KeyStage KeyStage { get; set; } = null!;
    public string Name { get; set; } = null!;

    protected bool Equals(Subject other)
    {
        return Id == other.Id && KeyStageId == other.KeyStageId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Subject)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, KeyStageId);
    }
}
