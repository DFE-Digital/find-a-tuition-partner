namespace Application.Common.DTO;

public class NotificationsRecipientDto
{
    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;
    public Dictionary<string, dynamic> Personalisation { get; set; } = null!;
}