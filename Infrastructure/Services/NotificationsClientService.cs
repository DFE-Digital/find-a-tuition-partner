using Application.Common.DTO;
using Application.Common.Interfaces;
using Domain.Enums;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Interfaces;

namespace Infrastructure.Services;

public class NotificationsClientService : INotificationsClientService
{
    private readonly IAsyncNotificationClient _notificationClient;
    private readonly ILogger<NotificationsClientService> _logger;
    private readonly GovUkNotifyOptions _config;

    public NotificationsClientService(IOptions<GovUkNotifyOptions> config, ILogger<NotificationsClientService> logger,
        IAsyncNotificationClient notificationClient)
    {
        _config = config.Value;
        _logger = logger;
        _notificationClient = notificationClient;
    }

    public async Task<bool> SendEmailAsync(IEnumerable<NotificationsRecipientDto> notificationsRecipients, EmailTemplateType emailTemplateType)
    {
        notificationsRecipients = notificationsRecipients.ToList();

        var testEmail = _config.TestEmailAddress;

        if (!string.IsNullOrEmpty(testEmail))
        {
            var recipients = testEmail.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            notificationsRecipients = notificationsRecipients.Take(recipients.Count).ToList();

            for (var i = 0; i < recipients.Count; i++)
            {
                notificationsRecipients.ToList()[i].Email = recipients[i];
            }
        }

        foreach (var recipient in notificationsRecipients.Where(x => string.IsNullOrEmpty(x.Email)))
        {
            _logger.LogError("No email address was supplied for the recipient: {recipient}.", recipient);

        }

        var emailTemplateId = GetEmailTemplateId(emailTemplateType, _config);

        if (string.IsNullOrEmpty(emailTemplateId))
        {
            _logger.LogError("No templateId was supplied for the {emailType}.", emailTemplateType);
            return false;
        }

        try
        {
            foreach (var recipient in notificationsRecipients.Where(recipient => !string.IsNullOrEmpty(recipient.Email)))
            {
                _logger.LogInformation("Preparing to send to {target}", recipient.Email);

                var result = await _notificationClient.SendEmailAsync(recipient.Email,
                    emailTemplateId, personalisation: recipient.Personalisation);

                _logger.LogInformation("Email successfully sent to: {email}", recipient.Email);
                _logger.LogInformation("Result: {id} {reference} {uri}", result.id, result.reference, result.uri);
                _logger.LogInformation("Result: {content}", result.content);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred while attempting to SendEmailAsync: {ex}", ex);
        }

        return false;
    }

    private static string GetEmailTemplateId(EmailTemplateType emailTemplateType, GovUkNotifyOptions config)
    {
        return emailTemplateType switch
        {
            EmailTemplateType.Enquiry => !string.IsNullOrEmpty(config.EnquiryTemplateId)
                ? config.EnquiryTemplateId
                : string.Empty,
            EmailTemplateType.EnquirerViewResponses => !string.IsNullOrEmpty(config.EnquirerViewResponsesTemplateId)
                ? config.EnquirerViewResponsesTemplateId
                : string.Empty,
            _ => string.Empty
        };
    }
}