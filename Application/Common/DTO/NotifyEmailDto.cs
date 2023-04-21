using Domain.Enums;

namespace Application.Common.DTO;

public class NotifyEmailDto
{
    public NotifyEmailDto()
    {
        PersonalisationPropertiesToAmalgamate = new List<string>();
        Personalisation = new Dictionary<string, dynamic>();
        NotifyResponse = new NotifyResponseDto();
    }

    public string Email { get; set; } = null!;

    public string EmailAddressUsedForTesting { get; set; } = null!; //TODO - possibly remove, although how know to add testing comment in NotificationsClientService if remove it?

    public Dictionary<string, dynamic> Personalisation { get; set; } = null!;

    public List<string> PersonalisationPropertiesToAmalgamate { get; set; } = null!; //TODO - remove this

    public string ClientReference { get; set; } = null!;

    public EmailTemplateType EmailTemplateType { get; set; }

    public string ClientReferenceIfAmalgamate { get; set; } = null!; //TODO - remove this

    public NotifyResponseDto NotifyResponse { get; set; }
}