namespace Domain;

public class TuitionType
{
    public int Id { get; set; }
    public string SeoUrl { get; set; } = null!;
    public string Name { get; set; } = null!;

    protected bool Equals(TuitionType other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TuitionType)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}