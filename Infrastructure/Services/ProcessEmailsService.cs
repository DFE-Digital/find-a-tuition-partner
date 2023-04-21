﻿using Application.Commands.Enquiry.Build;
using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Admin;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Infrastructure.Configuration;
using Infrastructure.Mapping.Configuration;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EmailStatus = Domain.Enums.EmailStatus;

namespace Infrastructure.Services;

public class ProcessEmailsService : IProcessEmailsService
{
    private const string ScheduleName = "Process Emails";
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddEnquiryCommandHandler> _logger;
    private readonly INotificationsClientService _notificationsClientService;
    private readonly EmailSettings _emailSettingsConfig;

    public ProcessEmailsService(IUnitOfWork unitOfWork,
        INotificationsClientService notificationsClientService,
        ILogger<AddEnquiryCommandHandler> logger,
        IOptions<EmailSettings> emailSettingsConfig)
    {
        _unitOfWork = unitOfWork;
        _notificationsClientService = notificationsClientService;
        _logger = logger;
        _emailSettingsConfig = emailSettingsConfig.Value;
    }

    //TODO - Consider if all email stuff should be in own project?
    public async Task<ProcessedEmailsModel> ProcessAllEmailsAsync()
    {
        var processedEmailsModel = new ProcessedEmailsModel();
        var startDate = DateTime.UtcNow;
        _logger.LogInformation("Starting ProcessAllEmailsAsync at {startDate}", startDate);

        try
        {
            var startedOK = await StartProcessing(startDate);
            if (startedOK)
            {
                await PollForStatusUpdatesAsync(processedEmailsModel);

                processedEmailsModel.EmailsSent = await SendEmailsToBeProcessedAsync(null, false);

                var finishedDate = DateTime.UtcNow;
                var finishedMessage = $"Completed email process successfully.  Started at {startDate}, finished at {finishedDate}, taking {(finishedDate - startDate).TotalSeconds} seconds.  " +
                    $"{processedEmailsModel.EmailsCheckStatus} email status checks, {processedEmailsModel.EmailsUpdatedStatus} changed email status, {processedEmailsModel.EmailsSent} emails sent.";

                await FinishProcessing(finishedDate, finishedMessage);

                processedEmailsModel.Outcome = finishedMessage;
            }
            else
            {
                processedEmailsModel.Outcome = $"Previous email process is still running at {startDate}.";
            }

            _logger.LogInformation(processedEmailsModel.Outcome);
        }
        catch (Exception ex)
        {
            await FinishProcessing(DateTime.UtcNow, $"Unexpected exception thrown. {ex}");
            throw;
        };

        return processedEmailsModel;
    }

    public async Task<int> SendEmailAsync(int emailLogId)
    {
        return await SendEmailsToBeProcessedAsync(new int[] { emailLogId });
    }

    public async Task<int> SendEmailsAsync(int[] emailLogIds)
    {
        return await SendEmailsToBeProcessedAsync(emailLogIds);
    }

    public string? GetEmailAddressUsedForTesting(string? emailToBeUsedIfTestingEnabled = null)
    {
        string? testingEmail = null;

        var overrideEmail = _emailSettingsConfig.OverrideAddress;

        if (!string.IsNullOrWhiteSpace(overrideEmail))
        {
            testingEmail = overrideEmail;
        }
        else if (!string.IsNullOrWhiteSpace(emailToBeUsedIfTestingEnabled) && _emailSettingsConfig.AllSentToEnquirer)
        {
            testingEmail = emailToBeUsedIfTestingEnabled;
        }

        return testingEmail;
    }


    private async Task<bool> StartProcessing(DateTime date)
    {
        var scheduledProcessingInfo = await _unitOfWork.ScheduledProcessingInfoRepository
            .SingleOrDefaultAsync(x => x.ScheduleName == ScheduleName);

        if (scheduledProcessingInfo == null)
        {
            _unitOfWork.ScheduledProcessingInfoRepository.AddAsync(new ScheduledProcessingInfo()
            {
                ScheduleName = ScheduleName,
                LastStartedDate = date
            });

            await _unitOfWork.Complete();
        }
        else
        {
            if (scheduledProcessingInfo.LastFinishedDate == null)
            {
                if (scheduledProcessingInfo.LastStartedDate.AddMinutes(30) < DateTime.UtcNow)
                {
                    _logger.LogError("Previous ProcessAllEmailsAsync has been running for more than 30 minutes.  Was last run {LastStartedDate}, starting a fresh!", scheduledProcessingInfo.LastStartedDate);
                }
                else
                {
                    if (scheduledProcessingInfo.LastStartedDate.AddMinutes(5) < DateTime.UtcNow)
                    {
                        _logger.LogWarning("Previous ProcessAllEmailsAsync has been running for more than 5 minutes.  Was last run {LastStartedDate}.", scheduledProcessingInfo.LastStartedDate);
                    }

                    return false;
                }
            }

            scheduledProcessingInfo.LastStartedDate = DateTime.UtcNow;
            scheduledProcessingInfo.LastFinishedDate = null;
            scheduledProcessingInfo.Status = null;

            _unitOfWork.ScheduledProcessingInfoRepository.Update(scheduledProcessingInfo);

            await _unitOfWork.Complete();
        }

        return true;
    }

    private async Task FinishProcessing(DateTime date, string outcomeMessage)
    {
        var scheduledProcessingInfo = await _unitOfWork.ScheduledProcessingInfoRepository
            .SingleOrDefaultAsync(x => x.ScheduleName == ScheduleName);

        scheduledProcessingInfo!.LastFinishedDate = date;
        scheduledProcessingInfo!.Status = outcomeMessage;

        _unitOfWork.ScheduledProcessingInfoRepository.Update(scheduledProcessingInfo);

        await _unitOfWork.Complete();
    }

    private async Task PollForStatusUpdatesAsync(ProcessedEmailsModel processedEmailsModel)
    {

        /*
            Pre-sending processing:
                1) Update all currently in status that has flag PollForStatusUpdateIfSent = true and LastEmailSendAttemptDate < now and FinishProcessingDate > now.  
                Only if status has changed do the following:
                    a) Update status and LastStatusChangedDate = now
                    b) If new status is NotifyDelivered see if any chained emails (in EmailTriggerActivation and status WaitingToBeTriggered).  If so update these ProcessFromDate = now
                    c) If the new status is failure (If RetrySendInSeconds != null) then update the ProcessFromDate = now.AddSeconds(RetrySendInSeconds)
         */

        var emailsToPollForStatusUpdate = await _unitOfWork.EmailLogRepository
            .GetAllAsync(x => (x.EmailStatus.PollForStatusUpdateIfSent &&
                                x.LastEmailSendAttemptDate != null &&
                                x.LastEmailSendAttemptDate < DateTime.UtcNow &&
                                x.FinishProcessingDate > DateTime.UtcNow),
            "EmailStatus,EmailNotifyResponseLog,EmailsActivatedByThisEmail,EmailsActivatedByThisEmail.ActivateEmailLog",
            true);

        if (emailsToPollForStatusUpdate != null && emailsToPollForStatusUpdate.Any())
        {
            var emailStatuses = _unitOfWork.EmailStatusRepository.GetAll();

            foreach (var emailLog in emailsToPollForStatusUpdate)
            {
                try
                {
                    if (emailLog == null || emailLog.EmailNotifyResponseLog == null || emailLog.EmailStatus == null)
                    {
                        throw new InvalidDataException("The emailsToPollForStatusUpdate has an unexpected empty data");
                    }

                    processedEmailsModel.EmailsCheckStatus++;
                    var currentStatus = emailLog.EmailStatus.Status.GetEnumFromDisplayName<EmailStatus>();

                    var newStatus = await _notificationsClientService.GetEmailStatus(emailLog!.EmailNotifyResponseLog!.NotifyId!);

                    if (currentStatus != newStatus)
                    {
                        _logger.LogInformation("Email ref {ClientReferenceNumber} status update from {currentStatus}, to {newStatus}.",
                            emailLog.ClientReferenceNumber,
                            currentStatus.DisplayName(),
                            newStatus.DisplayName());

                        emailLog.EmailStatusId = (int)newStatus;
                        emailLog.LastStatusChangedDate = DateTime.UtcNow;

                        if (newStatus == EmailStatus.NotifyDelivered && emailLog!.EmailsActivatedByThisEmail != null && emailLog!.EmailsActivatedByThisEmail.Any())
                        {
                            _logger.LogInformation("Email ref {ClientReferenceNumber} is delivered, triggering activation of {EmailsActivatedByThisEmailCount} emails.",
                                emailLog.ClientReferenceNumber,
                                emailLog!.EmailsActivatedByThisEmail.Count);

                            foreach (var emailToActivate in emailLog!.EmailsActivatedByThisEmail)
                            {
                                emailToActivate.ActivateEmailLog.ProcessFromDate = DateTime.UtcNow;
                            }
                        }

                        var newStatusEntity = emailStatuses.Single(x => x.Id == emailLog.EmailStatusId);
                        if (newStatusEntity!.RetrySendInSeconds != null)
                        {
                            var retryInSeconds = newStatusEntity!.RetrySendInSeconds!;
                            DateTime retryDateTime = DateTime.UtcNow.AddSeconds((double)retryInSeconds);

                            _logger.LogInformation("Email ref {ClientReferenceNumber} status {newStatus}, so set to retry in {retryInSeconds} seconds, on {retryDateTime}.",
                                emailLog.ClientReferenceNumber,
                                 newStatus.DisplayName(),
                                retryInSeconds,
                                retryDateTime);

                            emailLog.ProcessFromDate = retryDateTime;
                        }

                        _unitOfWork.EmailLogRepository.Update(emailLog);

                        processedEmailsModel.EmailsUpdatedStatus++;
                    }
                }
                catch (Exception ex)
                {
                    //We don't want to throw the error here, we log and continue processing other emails
                    _logger.LogError(ex, "Unexpected error in emailsToPollForStatusUpdate for EmailLog.ClientReferenceNumber: {ClientReferenceNumber}, EmailLog.Id: {EmailLogId}", emailLog.ClientReferenceNumber, emailLog.Id);
                }
            }
        }
    }

    private async Task<int> SendEmailsToBeProcessedAsync(int[]? emailLogIds = null, bool throwExceptions = true)
    {
        /*
            Send emails:
                Send if the status has flag AllowEmailSending = true and ProcessFromDate not null and ProcessFromDate <= now and FinishProcessingDate > now
                    Update the LastEmailSendAttemptDate = now, the status to BeenProcessed and LastStatusChangedDate = now
                    Update the EmailNotifyLog with Notify response details and clear any previous ExceptionCode & ExceptionMessage - if retry then need to ensure that the Archive has previous notify info.
                If error then log in EmailNotifyLog.ExceptionCode & EmailNotifyLog.ExceptionMessage
                    Set status to ProcessingFailure and the ProcessFromDate = now.AddSeconds(RetrySendInSeconds)
         */

        var emailsSent = 0;

        var emailsToSend = await _unitOfWork.EmailLogRepository
            .GetAllAsync(x => (x.EmailStatus.AllowEmailSending &&
                                x.ProcessFromDate != null &&
                                x.ProcessFromDate <= DateTime.UtcNow &&
                                x.FinishProcessingDate > DateTime.UtcNow &&
                                (emailLogIds == null ||
                                emailLogIds.Count() == 0 ||
                                emailLogIds.Contains(x.Id))),
            "EmailStatus,EmailPersonalisationLogs",
            true);

        if (emailsToSend.Any())
        {
            var notifyEmails = emailsToSend.Select(x => new NotifyEmailDto()
            {
                Email = string.IsNullOrWhiteSpace(x.EmailAddressUsedForTesting) ? x.EmailAddress : x.EmailAddressUsedForTesting,
                ClientReference = x.ClientReferenceNumber,
                EmailTemplateType = x.EmailTemplateShortName.GetEnumFromDisplayName<EmailTemplateType>(),
                Personalisation = x.EmailPersonalisationLogs == null ?
                    new Dictionary<string, dynamic>() :
                    x.EmailPersonalisationLogs!.Select(y => new KeyValuePair<string, dynamic>(y.Key, y.Value)).ToDictionary(x => x.Key, x => x.Value)
            }).ToList();

            try
            {
                await _notificationsClientService.SendEmailAsync(notifyEmails);
            }
            catch
            {
                if (throwExceptions)
                {
                    await UpdateSentEmailsAsync(emailsToSend, notifyEmails);
                    throw;
                }
            }

            emailsSent = await UpdateSentEmailsAsync(emailsToSend, notifyEmails);
        }

        return emailsSent;
    }

    private async Task<int> UpdateSentEmailsAsync(IEnumerable<EmailLog> emailsToUpdate, IEnumerable<NotifyEmailDto> emailResults)
    {
        NotifyResponseMappingConfig.Configure();

        var sentEmails = 0;
        foreach (var emailResult in emailResults)
        {
            EmailLog? emailToUpdate = null;
            try
            {
                emailToUpdate = emailsToUpdate.Single(x => x.ClientReferenceNumber == emailResult.ClientReference);

                var notifyResponse = emailToUpdate.EmailNotifyResponseLog == null ? new EmailNotifyResponseLog() : emailToUpdate.EmailNotifyResponseLog;

                //TODO - Check clears data on map if switch from error to success
                notifyResponse = emailResult.NotifyResponse.Adapt(notifyResponse);

                emailToUpdate.EmailNotifyResponseLog = notifyResponse;

                var oldStatus = emailToUpdate.EmailStatusId;

                if (!string.IsNullOrEmpty(notifyResponse.NotifyId))
                {
                    emailToUpdate.EmailStatusId = (int)EmailStatus.BeenProcessed;
                    sentEmails++;
                }
                else
                {
                    emailToUpdate.EmailStatusId = (int)EmailStatus.ProcessingFailure;
                    var retryInSeconds = _unitOfWork.EmailStatusRepository.GetById(emailToUpdate.EmailStatusId).RetrySendInSeconds;
                    emailToUpdate.ProcessFromDate = DateTime.UtcNow.AddSeconds((double)retryInSeconds!);
                }

                emailToUpdate.LastEmailSendAttemptDate = DateTime.UtcNow;
                if (oldStatus != emailToUpdate.EmailStatusId)
                {
                    emailToUpdate.LastStatusChangedDate = DateTime.UtcNow;
                }

                _unitOfWork.EmailLogRepository.Update(emailToUpdate);

                await _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                //We don't want to throw the error here, we log and continue processing other emails
                _logger.LogError(ex, "Unexpected error in UpdateSentEmailsAsync for EmailLog.ClientReferenceNumber: {ClientReferenceNumber}, EmailLog.Id: {EmailLogId}",
                    emailToUpdate == null ? "NULL" : emailToUpdate.ClientReferenceNumber,
                    emailToUpdate == null ? "NULL" : emailToUpdate.Id);
            }
        }

        return sentEmails;
    }
}