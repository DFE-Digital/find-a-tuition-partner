using Application.Commands.Enquiry.Build;
using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql.Replication;

namespace Infrastructure.Services;

public class ProcessEmailsService : IProcessEmailsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddEnquiryCommandHandler> _logger;

    public ProcessEmailsService(IUnitOfWork unitOfWork, ILogger<AddEnquiryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /*
     * TODO...
     * 
    Emails processing:
        Add emails to EmailLog table
            - Enquiry submitted (in AddEnquiryCommandHandler):
                - Enquirer email -> Status = NewToBeProcessed, ProcessFromDate = now
                - TP email -> Status = WaitingToBeTriggered, ProcessFromDate = NULL -> add as linked (EmailTriggerActivation) to send on enquirer email
                    - Config flag if call TP email straight away then Status = NewToBeProcessed, ProcessFromDate = now and not added to EmailTriggerActivation
                - Call processing for Enquirer email (using ref number) straight away
            - TP responds (in AddEnquiryResponseCommandHandler):
                - TP email -> Status = NewToBeProcessed, ProcessFromDate = now
                - Enquirer email -> Status = DelayedEmail, ProcessFromDate = next morning (TBC)
                    - Consider how this works for batching, may need to update existing emaillog to a different template if there are multiples (consider what happens in terms of personalisation - do we need to show all TP names in email)?
                         - Config flag to drive if use batching or not
                - Call processing for TP email ref number straight away
            - NOTE: will need to move the almgamated (and maybe testing replace emails & content etc?) logic out of NotificationsClientService so only store the correct number of EmailLogs?

    Process emails (every min from CURL or via code straight after added above - allow a ClientReferenceNumber to be passed in so can process a single email)
    Pre-sending processing:
        1) Update all currently in status that has flag PollForStatusUpdateIfSent = true and LastEmailSendAttemptDate < now and FinishProcessingDate > now.  Only if status has changed do the following:
            a) If new status is NotifyDelivered see if any chained emails (in EmailTriggerActivation and status WaitingToBeTriggered).  If so update these ProcessFromDate = now
            b) If the new status is failure (If RetrySendInSeconds != null) then update the ProcessFromDate = now.AddSeconds(RetrySendInSeconds)
                NOTE: can we get any error info to log via client?
            c) Update LastStatusChangedDate = now

    Send emails:
        Send if the status has flag AllowEmailSending = true and ProcessFromDate not null and ProcessFromDate <= now and FinishProcessingDate > now
            Update the LastEmailSendAttemptDate = now, the status to BeenProcessed and LastStatusChangedDate = now
            Update the EmailNotifyLog with Notify response details and clear any previous ExceptionCode & ExceptionMessage - if retry then need to ensure that the Archive has previous notify info.
        If error then log in EmailNotifyLog.ExceptionCode & EmailNotifyLog.ExceptionMessage -> TBC, what if this is a retry and has previous NotiftyId etc?
            Set status to ProcessingFailure and the ProcessFromDate = now.AddSeconds(RetrySendInSeconds)
    */


    public async Task ProcessAllEmails()
    {
        await PollForStatusUpdates();
        await SendEmailsToBeProcessed();
    }


    public async Task SendEmail(string clientReference)
    {
        //TODO - remove following, for testing only...
        var emailLogTestingDummyRecord = await _unitOfWork.EmailLogRepository
            .GetAllAsync(x => (x.Id == 1), "EmailStatus",
                true);

        var dummyRecordCount = emailLogTestingDummyRecord.Count();

        var dummyRecordEmailAddress = emailLogTestingDummyRecord.First().EmailAddress;

        var dummyRecordStatusId = emailLogTestingDummyRecord.First().EmailStatus.Id;


        await SendEmailsToBeProcessed(clientReference);
    }


    private async Task PollForStatusUpdates()
    {
        var emailsToPollForStatusUpdate = await _unitOfWork.EmailLogRepository
            .GetAllAsync(x => (x.EmailStatus.PollForStatusUpdateIfSent &&
                                x.LastEmailSendAttemptDate != null &&
                                x.LastEmailSendAttemptDate < DateTime.UtcNow &&
                                x.FinishProcessingDate > DateTime.UtcNow), "EmailStatus",
                true);


    }

    private async Task SendEmailsToBeProcessed(string? clientReference = null)
    {
        var emailsToSend = await _unitOfWork.EmailLogRepository
            .GetAllAsync(x => (x.EmailStatus.AllowEmailSending &&
                                x.ProcessFromDate != null &&
                                x.ProcessFromDate <= DateTime.UtcNow &&
                                x.FinishProcessingDate > DateTime.UtcNow &&
                                (string.IsNullOrEmpty(clientReference) ||
                                x.ClientReferenceNumber == clientReference)), "EmailStatus",
                true);


    }

}
