using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Extensions;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Configuration;
using Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using EmailStatus = Domain.Enums.EmailStatus;

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

    public NotificationsClientService(IOptions<GovUkNotifyOptions> notifyConfig,
        IOptions<EmailSettings> emailSettingsConfig,
        ILogger<NotificationsClientService> logger,
        IAsyncNotificationClient notificationClient,
        IHostEnvironment hostEnvironment)
    {
        _notifyConfig = notifyConfig.Value;
        _emailSettingsConfig = emailSettingsConfig.Value;
        _logger = logger;
        _notificationClient = notificationClient;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<bool> SendEmailAsync(NotifyEmailDto notifyEmail, bool includeChangedFromEmailAddress = true)
    {
        var clientReference = notifyEmail.ClientReference;

        var emailTemplateId = GetEmailTemplateId(notifyEmail.EmailTemplateType, _notifyConfig);

        AddTestingInformation(notifyEmail, includeChangedFromEmailAddress);

        try
        {
            _logger.LogInformation("Preparing to send, Notify client ref: {clientReference}", clientReference);

            var result = await _notificationClient.SendEmailAsync(notifyEmail.Email,
                emailTemplateId, personalisation: notifyEmail.Personalisation, clientReference);

            notifyEmail.NotifyResponse.NotifyId = result.id;
            notifyEmail.NotifyResponse.Reference = result.reference;
            notifyEmail.NotifyResponse.Uri = result.uri;
            notifyEmail.NotifyResponse.TemplateId = result.template.id;
            notifyEmail.NotifyResponse.TemplateUri = result.template.uri;
            notifyEmail.NotifyResponse.TemplateVersion = result.template.version;
            notifyEmail.NotifyResponse.EmailResponseContentFrom = result.content.fromEmail;
            notifyEmail.NotifyResponse.EmailResponseContentBody = result.content.body;
            notifyEmail.NotifyResponse.EmailResponseContentSubject = result.content.subject;

            _logger.LogInformation("Email successfully sent, Notify client ref: {clientReference}.  Result details: Id: {id}; Ref: {reference}; URI: {uri}; Content: {content}",
                clientReference, result.id, result.reference, result.uri, result.content);

            return true;
        }
        catch (Exception ex)
        {
            notifyEmail.NotifyResponse.ExceptionCode = ex.Message.GetGovNotifyStatusCodeFromExceptionMessage().ToString();
            notifyEmail.NotifyResponse.ExceptionMessage = ex.ToString();

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

    public async Task<bool> SendEmailAsync(IEnumerable<NotifyEmailDto> notifyEmails)
    {
        notifyEmails = notifyEmails.ToList();

        var allEmailsSent = true;

        //See if we need to amalgamate multiuple emails for testing purposes
        if (_emailSettingsConfig.AmalgamateResponses && notifyEmails.Count() > 1 && notifyEmails.First().PersonalisationPropertiesToAmalgamate.Count > 0)
        {
            allEmailsSent = await AmalgamateEmailForTesting(notifyEmails);
        }
        else
        {
            // By using Task.WhenAll and the Select LINQ method, we can now process and send emails in parallel,
            // which can significantly improve performance when dealing with multiple recipients
            var sendEmailTasks = notifyEmails
                .Where(recipient => !string.IsNullOrEmpty(recipient.Email))
                .Select(async recipient =>
                {
                    return await SendEmailAsync(recipient);
                })
                .ToList();

            var results = await Task.WhenAll(sendEmailTasks);

            allEmailsSent = results.All(result => result);
        }

        return allEmailsSent;
    }

    public async Task<EmailStatus> GetEmailStatus(string notificationId)
    {
        try
        {
            var result = await _notificationClient.GetNotificationByIdAsync(notificationId);
            return result.status.GetEnumFromDisplayName<EmailStatus>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error has occurred while attempting to GetNotificationById, notificationId: {notificationId}", notificationId);
            throw;
        }
    }

    private void AddTestingInformation(NotifyEmailDto notifyEmail, bool includeChangedFromEmailAddress = true)
    {
        //Add in these keys as empty since must exist in code even if nothing to pass in
        AddDefaultTestPersonalisation(notifyEmail.Personalisation);

        var overrideEmail = _emailSettingsConfig.OverrideAddress;
        var overrideExtraInfoText = "overridden to use";
        if (string.IsNullOrWhiteSpace(overrideEmail) && _emailSettingsConfig.AllSentToEnquirer)
        {
            overrideEmail = notifyEmail.EmailAddressUsedForTesting;
            overrideExtraInfoText = "changed to";
        }

        if (!string.IsNullOrWhiteSpace(overrideEmail) && !string.Equals(notifyEmail.Email, overrideEmail, StringComparison.InvariantCultureIgnoreCase))
        {
            var extraInfoChangedFrom = includeChangedFromEmailAddress ? $" rather than {notifyEmail.Email}" : string.Empty;

            notifyEmail.Email = overrideEmail;

            AddPersonalisation(notifyEmail.Personalisation, TestExtraInfoKey, $"For testing purposes the email has been {overrideExtraInfoText} {overrideEmail}{extraInfoChangedFrom}.", true);
        }

        if (!_hostEnvironment.IsProduction())
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrWhiteSpace(environmentName))
                AddPersonalisation(notifyEmail.Personalisation, TestWebsiteEnvName, $"**** {environmentName} NTP email ****  ");
        }
    }

    private async Task<bool> AmalgamateEmailForTesting(IEnumerable<NotifyEmailDto> notifyEmails)
    {
        var initialRecipient = notifyEmails.First();
        var keys = new List<string>(initialRecipient.Personalisation.Keys);
        foreach (string key in keys)
        {
            if (initialRecipient.PersonalisationPropertiesToAmalgamate.Contains(key))
            {
                initialRecipient.Personalisation[key] = $"{initialRecipient.Email} - {initialRecipient.Personalisation[key]}";
            }
        }

        for (int i = 1; i < notifyEmails.Count(); i++)
        {
            foreach (var personalisation in notifyEmails.ElementAt(i).Personalisation)
            {
                if (initialRecipient.PersonalisationPropertiesToAmalgamate.Contains(personalisation.Key))
                {
                    AddPersonalisation(initialRecipient.Personalisation, personalisation.Key, $"{notifyEmails.ElementAt(i).Email} - {personalisation.Value}", true, false);
                }
            }
        }

        AddPersonalisation(initialRecipient.Personalisation, TestExtraInfoKey, $"This is an amalgamated email for testing purposes.", true);

        initialRecipient.ClientReference = initialRecipient.ClientReferenceIfAmalgamate;

        return await SendEmailAsync(initialRecipient, false);
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