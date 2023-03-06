using Application.Common.DTO;
using Application.Common.Interfaces;
using Domain.Enums;
using Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Interfaces;

namespace Infrastructure.Services;

public class NotificationsClientService : INotificationsClientService
{
    private const string TestWebsiteEnvName = "test_website_env_name";
    private const string TestExtraInfoKey = "test_extra_info";

    private readonly IAsyncNotificationClient _notificationClient;
    private readonly ILogger<NotificationsClientService> _logger;
    private readonly GovUkNotifyOptions _notifyConfig;
    private readonly EmailSettings _emailSettingsConfig;
    private readonly IHostEnvironment _hostEnvironment;

    public NotificationsClientService(IOptions<GovUkNotifyOptions> notifyConfig, IOptions<EmailSettings> emailSettingsConfig, ILogger<NotificationsClientService> logger,
        IAsyncNotificationClient notificationClient, IHostEnvironment hostEnvironment)
    {
        _notifyConfig = notifyConfig.Value;
        _emailSettingsConfig = emailSettingsConfig.Value;
        _logger = logger;
        _notificationClient = notificationClient;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<bool> SendEmailAsync(NotificationsRecipientDto notificationsRecipient, EmailTemplateType emailTemplateType, bool includeChangedFromEmailAddress = true)
    {
        if (string.IsNullOrWhiteSpace(notificationsRecipient.Email))
        {
            _logger.LogError("No email address was supplied for the recipient: {recipient}.", notificationsRecipient.Email);
        }

        var emailTemplateId = GetEmailTemplateId(emailTemplateType, _notifyConfig);

        if (string.IsNullOrEmpty(emailTemplateId))
        {
            _logger.LogError("No templateId was supplied for the {emailType}.", emailTemplateType);
            return false;
        }

        AddTestingInformation(notificationsRecipient, includeChangedFromEmailAddress);

        try
        {
            _logger.LogInformation("Preparing to send to {target}", notificationsRecipient.Email);

            var result = await _notificationClient.SendEmailAsync(notificationsRecipient.Email,
                emailTemplateId, personalisation: notificationsRecipient.Personalisation);

            _logger.LogInformation("Email successfully sent to: {email}", notificationsRecipient.Email);
            _logger.LogInformation("Result: {id} {reference} {uri}", result.id, result.reference, result.uri);
            _logger.LogInformation("Result: {content}", result.content);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred while attempting to SendEmailAsync: {ex}", ex);
        }

        return false;
    }

    public async Task<bool> SendEmailAsync(IEnumerable<NotificationsRecipientDto> notificationsRecipients, EmailTemplateType emailTemplateType)
    {
        notificationsRecipients = notificationsRecipients.ToList();

        foreach (var recipient in notificationsRecipients.Where(x => string.IsNullOrEmpty(x.Email)))
        {
            _logger.LogError("No email address was supplied for the recipient: {recipient}.", recipient);
        }

        try
        {
            var allEmailsSent = true;

            //See if we need to amalgamate multiuple emails for testing purposes
            if (_emailSettingsConfig.AmalgamateResponses && notificationsRecipients.Count() > 1)
            {
                allEmailsSent = await AmalgamateEmailForTesting(notificationsRecipients, emailTemplateType);
            }
            else
            {
                foreach (var recipient in notificationsRecipients.Where(recipient => !string.IsNullOrEmpty(recipient.Email)))
                {
                    var emailSent = await SendEmailAsync(recipient, emailTemplateType);
                    allEmailsSent = !allEmailsSent ? allEmailsSent : emailSent;
                }
            }

            return allEmailsSent;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred while attempting to SendEmailAsync: {ex}", ex);
        }

        return false;
    }

    private void AddTestingInformation(NotificationsRecipientDto notificationsRecipient, bool includeChangedFromEmailAddress = true)
    {
        //Add in these keys as empty since must exist in code even if nothing to pass in
        AddDefaultTestPersonalisation(notificationsRecipient.Personalisation);

        var overrideEmail = _emailSettingsConfig.OverrideAddress;
        var overrideExtraInfoText = "overridden to use";
        if (string.IsNullOrWhiteSpace(overrideEmail) && _emailSettingsConfig.AllSentToEnquirer)
        {
            overrideEmail = notificationsRecipient.EnquirerEmailForTestingPurposes;
            overrideExtraInfoText = "changed to the enquirer";
        }

        if (!string.IsNullOrWhiteSpace(overrideEmail) && !string.Equals(notificationsRecipient.Email, overrideEmail, StringComparison.InvariantCultureIgnoreCase))
        {
            var extraInfoChangedFrom = includeChangedFromEmailAddress ? $" rather than {notificationsRecipient.Email}" : string.Empty;

            notificationsRecipient.Email = overrideEmail;

            AddPersonalisation(notificationsRecipient.Personalisation, TestExtraInfoKey, $"For testing purposes the email has been {overrideExtraInfoText} {overrideEmail}{extraInfoChangedFrom}.", true);
        }

        if (!_hostEnvironment.IsProduction())
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrWhiteSpace(environmentName))
                AddPersonalisation(notificationsRecipient.Personalisation, TestWebsiteEnvName, $"**** {environmentName} NTP email ****  ");
        }
    }

    private async Task<bool> AmalgamateEmailForTesting(IEnumerable<NotificationsRecipientDto> notificationsRecipients, EmailTemplateType emailTemplateType)
    {
        var initialRecipient = notificationsRecipients.First();
        var keys = new List<string>(initialRecipient.Personalisation.Keys);
        foreach (string key in keys)
        {
            initialRecipient.Personalisation[key] = $"{initialRecipient.Email} - {initialRecipient.Personalisation[key]}";
        }

        for (int i = 1; i < notificationsRecipients.Count(); i++)
        {
            foreach (var personalisation in notificationsRecipients.ElementAt(i).Personalisation)
            {
                AddPersonalisation(initialRecipient.Personalisation, personalisation.Key, $"{notificationsRecipients.ElementAt(i).Email} - {personalisation.Value}", true, false);
            }
        }

        AddPersonalisation(initialRecipient.Personalisation, TestExtraInfoKey, $"This is an amalgamated email for testing purposes.", true);

        return await SendEmailAsync(initialRecipient, emailTemplateType, false);
    }

    private static string GetEmailTemplateId(EmailTemplateType emailTemplateType, GovUkNotifyOptions notifyConfig)
    {
        return emailTemplateType switch
        {
            EmailTemplateType.EnquirySubmittedConfirmationToEnquirer =>
                !string.IsNullOrEmpty(notifyConfig.TemplateIdEnquirySubmittedConfirmationToEnquirer)
                ? notifyConfig.TemplateIdEnquirySubmittedConfirmationToEnquirer
                : string.Empty,
            EmailTemplateType.EnquirySubmittedToTp => !string.IsNullOrEmpty(notifyConfig.TemplateIdEnquirySubmittedToTp)
                ? notifyConfig.TemplateIdEnquirySubmittedToTp
                : string.Empty,
            EmailTemplateType.EnquiryResponseReceivedConfirmationToEnquirer =>
                !string.IsNullOrEmpty(notifyConfig.TemplateIdEnquiryResponseReceivedConfirmationToEnquirer)
                ? notifyConfig.TemplateIdEnquiryResponseReceivedConfirmationToEnquirer
                : string.Empty,
            _ => string.Empty
        };
    }

    private static void AddDefaultTestPersonalisation(Dictionary<string, dynamic> personalisation)
    {
        if (!personalisation.ContainsKey(TestWebsiteEnvName))
        {
            personalisation.Add(TestWebsiteEnvName, string.Empty);
        }

        if (!personalisation.ContainsKey(TestExtraInfoKey))
        {
            personalisation.Add(TestExtraInfoKey, string.Empty);
        }
    }

    private static void AddPersonalisation(Dictionary<string, dynamic> personalisation, string key, dynamic value, bool addNewLine = false, bool addAsPrefix = true)
    {
        personalisation ??= new Dictionary<string, dynamic>();

        if (personalisation.ContainsKey(key))
        {
            var existingValue = personalisation[key];
            if (addAsPrefix)
            {
                personalisation[key] = addNewLine ? $"{value}{Environment.NewLine}{Environment.NewLine}{existingValue}" : $"{value}  {existingValue}";
            }
            else
            {
                personalisation[key] = addNewLine ? $"{existingValue}{Environment.NewLine}{Environment.NewLine}{value}" : $"{existingValue}  {value}";
            }
        }
        else
        {
            personalisation.Add(key, addNewLine ? $"{value}{Environment.NewLine}{Environment.NewLine}" : $"{value}");
        }
    }
}