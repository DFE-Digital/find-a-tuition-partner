using Application.Common.DTO;
using Domain.Enums;

namespace Application.Common.Interfaces;

public interface INotificationsClientService
{
    Task<bool> SendEmailAsync(NotificationsRecipientDto notificationsRecipient, EmailTemplateType emailTemplateType,
        string supportReferenceNumber, bool includeChangedFromEmailAddress = true);
    Task<bool> SendEmailAsync(IEnumerable<NotificationsRecipientDto> notificationsRecipients,
        EmailTemplateType emailTemplateType, string supportReferenceNumber);
}