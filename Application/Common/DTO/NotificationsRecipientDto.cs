namespace Application.Common.DTO;

public class NotificationsRecipientDto
{
    public NotificationsRecipientDto()
    {
        PersonalisationPropertiesToAmalgamate = new List<string>();
    }

    public string Email { get; set; } = null!;

    public string EnquirerEmailForTestingPurposes { get; set; } = null!;

    public string? Token { get; set; } = null!;

    public Dictionary<string, dynamic> Personalisation { get; set; } = null!;

    public List<string> PersonalisationPropertiesToAmalgamate { get; set; } = null!;
}