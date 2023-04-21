using Application.Common.DTO;
using EmailStatus = Domain.Enums.EmailStatus;

namespace Application.Common.Interfaces;

public interface INotificationsClientService
{
    Task<bool> SendEmailAsync(NotifyEmailDto notifyEmail, bool includeChangedFromEmailAddress = true);
    Task<bool> SendEmailAsync(IEnumerable<NotifyEmailDto> notifyEmails);

    Task<EmailStatus> GetEmailStatus(string notificationId);
}