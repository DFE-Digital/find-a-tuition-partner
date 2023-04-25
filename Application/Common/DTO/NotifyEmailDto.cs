using Domain.Enums;

namespace Application.Common.DTO;

public class NotifyEmailDto
{
    public NotifyEmailDto()
    {
        Personalisation = new Dictionary<string, dynamic>();
        NotifyResponse = new NotifyResponseDto();
    }

    public string Email { get; set; } = null!;

    public string? EmailAddressUsedForTesting { get; set; } = null!;

    public Dictionary<string, dynamic> Personalisation { get; set; } = null!;

    public string ClientReference { get; set; } = null!;

    public EmailTemplateType EmailTemplateType { get; set; }

    public NotifyResponseDto NotifyResponse { get; set; }
}