using System.Net;
using Application.Common.DTO;
using Domain.Enums;

namespace Application.Common.Interfaces;

public interface INotificationsClientService
{
    Task<(bool, HttpStatusCode)> SendEmailAsync(NotificationsRecipientDto notificationsRecipient, EmailTemplateType emailTemplateType,
        string supportReferenceNumber, bool includeChangedFromEmailAddress = true);
    Task<(bool, HttpStatusCode)> SendEmailAsync(IEnumerable<NotificationsRecipientDto> notificationsRecipients,
        EmailTemplateType emailTemplateType, string supportReferenceNumber);
}