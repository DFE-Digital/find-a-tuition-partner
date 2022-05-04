namespace Domain;

public class UserSession
{
    public UserSession()
    {
        Searches = new List<UserSearch>();
    }

    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public ICollection<UserSearch> Searches { get; set; }
}