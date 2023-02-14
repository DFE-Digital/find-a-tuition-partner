using Application.Common.DTO;
using Domain.Enums;

namespace Application.Common.Interfaces;

public interface INotificationsClientService
{
    Task<bool> SendEmailAsync(IEnumerable<NotificationsRecipientDto> notificationsRecipients, EmailTemplateType emailTemplateType);
}