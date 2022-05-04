namespace Domain;

public class UserSearch
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string SearchJson { get; set; } = null!;
}