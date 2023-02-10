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

    public async Task SendEmailAsync(List<string> recipients, EmailTemplateType emailTemplateType,
        Dictionary<string, dynamic> personalisation)
    {
        var testEmail = _config.TestEmailAddress;

        if (recipients.Any() && !string.IsNullOrEmpty(testEmail))
        {
            recipients = testEmail.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        if (!recipients.Any())
        {
            _logger.LogError("No email address was supplied for the recipient.");
            return;
        }

        var emailTemplateId = GetEmailTemplateId(emailTemplateType, _config);

        if (string.IsNullOrEmpty(emailTemplateId))
        {
            _logger.LogError("No templateId was supplied for the {emailType}.", emailTemplateType);
            return;
        }

        try
        {
            foreach (var recipient in recipients)
            {
                _logger.LogInformation("Preparing to send to {target}", recipient);

                var result = await _notificationClient.SendEmailAsync(recipient,
                    emailTemplateId, personalisation: personalisation);

                _logger.LogInformation("Email successfully sent to: {email}", recipient);
                _logger.LogInformation("Result: {id} {reference} {uri}", result.id, result.reference, result.uri);
                _logger.LogInformation("Result: {content}", result.content);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred while attempting to SendEmailAsync: {ex}", ex);
        }
    }

    private static string GetEmailTemplateId(EmailTemplateType emailTemplateType, GovUkNotifyOptions config)
    {
        if (emailTemplateType == EmailTemplateType.Enquiry)
        {
            return !string.IsNullOrEmpty(config.EnquiryTemplateId) ? config.EnquiryTemplateId : string.Empty;
        }

        return string.Empty;
    }
}