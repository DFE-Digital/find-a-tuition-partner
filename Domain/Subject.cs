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


public class Employee
{
    public string? FirstName { get; set; }
    public virtual EmployeeSkills GetSkills()
    {
        return new EmployeeSkills
        {
            CanSendEmails = true
        };
    }
}

public class EmployeeSkills
{
    public bool CanSendEmails { get; set; }
}

public class DeveloperSkills : EmployeeSkills
{
    public bool KnowsDotNet { get; set; }
}

public class Developer : Employee
{
    public override EmployeeSkills GetSkills()
    {
        return new DeveloperSkills
        {
            CanSendEmails = true,
            KnowsDotNet = true
        };
    }
}