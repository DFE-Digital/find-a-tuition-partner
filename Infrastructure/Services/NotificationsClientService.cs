using Application.Common.DTO;
using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Configuration;
using Infrastructure.Extensions;
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

    public async Task<bool> SendEmailAsync(NotificationsRecipientDto notificationsRecipient, EmailTemplateType emailTemplateType,
        bool includeChangedFromEmailAddress = true)
    {
        var clientReference = notificationsRecipient.ClientReference;

        var emailTemplateId = GetEmailTemplateId(emailTemplateType, _notifyConfig);

        AddTestingInformation(notificationsRecipient, includeChangedFromEmailAddress);

        try
        {
            _logger.LogInformation("Preparing to send, Notify client ref: {clientReference}", clientReference);

            var result = await _notificationClient.SendEmailAsync(notificationsRecipient.Email,
                emailTemplateId, personalisation: notificationsRecipient.Personalisation, clientReference);

            _logger.LogInformation("Email successfully sent, Notify client ref: {clientReference}.  Result details: Id: {id}; Ref: {reference}; URI: {uri}; Content: {content}",
                clientReference, result.id, result.reference, result.uri, result.content);

            return true;
        }
        catch (Exception ex)
        {
            if (ex.IsNonCriticalNotifyException())
            {
                _logger.LogWarning(ex, "A non critical Notify error has occurred while attempting to SendEmailAsync, email ref: {clientReference}", clientReference);
                throw new EmailSendException(ex.Message, ex);
            }
            else
            {
                _logger.LogError(ex, "An unexpected error has occurred while attempting to SendEmailAsync, email ref: {clientReference}", clientReference);
                throw;
            }
        }
    }

    public async Task<bool> SendEmailAsync(IEnumerable<NotificationsRecipientDto> notificationsRecipients,
        EmailTemplateType emailTemplateType)
    {
        notificationsRecipients = notificationsRecipients.ToList();

        var allEmailsSent = true;

        //See if we need to amalgamate multiuple emails for testing purposes
        if (_emailSettingsConfig.AmalgamateResponses && notificationsRecipients.Count() > 1 && notificationsRecipients.First().PersonalisationPropertiesToAmalgamate.Count > 0)
        {
            allEmailsSent = await AmalgamateEmailForTesting(notificationsRecipients, emailTemplateType);
        }
        else
        {
            // By using Task.WhenAll and the Select LINQ method, we can now process and send emails in parallel,
            // which can significantly improve performance when dealing with multiple recipients
            var sendEmailTasks = notificationsRecipients
                .Where(recipient => !string.IsNullOrEmpty(recipient.Email))
                .Select(async recipient =>
                {
                    return await SendEmailAsync(recipient, emailTemplateType);
                })
                .ToList();

            var results = await Task.WhenAll(sendEmailTasks);

            allEmailsSent = results.All(result => result);
        }

        return allEmailsSent;
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

    private async Task<bool> AmalgamateEmailForTesting(IEnumerable<NotificationsRecipientDto> notificationsRecipients,
        EmailTemplateType emailTemplateType)
    {
        var initialRecipient = notificationsRecipients.First();
        var keys = new List<string>(initialRecipient.Personalisation.Keys);
        foreach (string key in keys)
        {
            if (initialRecipient.PersonalisationPropertiesToAmalgamate.Contains(key))
            {
                initialRecipient.Personalisation[key] = $"{initialRecipient.Email} - {initialRecipient.Personalisation[key]}";
            }
        }

        for (int i = 1; i < notificationsRecipients.Count(); i++)
        {
            foreach (var personalisation in notificationsRecipients.ElementAt(i).Personalisation)
            {
                if (initialRecipient.PersonalisationPropertiesToAmalgamate.Contains(personalisation.Key))
                {
                    AddPersonalisation(initialRecipient.Personalisation, personalisation.Key, $"{notificationsRecipients.ElementAt(i).Email} - {personalisation.Value}", true, false);
                }
            }
        }

        AddPersonalisation(initialRecipient.Personalisation, TestExtraInfoKey, $"This is an amalgamated email for testing purposes.", true);

        initialRecipient.ClientReference = initialRecipient.ClientReferenceIfAmalgamate;

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
            EmailTemplateType.EnquiryResponseSubmittedConfirmationToTp =>
                !string.IsNullOrEmpty(notifyConfig.TemplateIdEnquiryResponseSubmittedConfirmationToTp)
                ? notifyConfig.TemplateIdEnquiryResponseSubmittedConfirmationToTp
                : string.Empty,
            EmailTemplateType.EmailVerification =>
                !string.IsNullOrEmpty(notifyConfig.TemplateIdEmailVerification)
                ? notifyConfig.TemplateIdEmailVerification
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

    private static void AddPersonalisation(Dictionary<string, dynamic> personalisation, string key, dynamic value,
        bool addNewLine = false, bool addAsPrefix = true)
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