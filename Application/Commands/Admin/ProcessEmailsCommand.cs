using Application.Common.Interfaces;
using Application.Common.Models.Admin;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Admin;

public record ProcessEmailsCommand : IRequest<ProcessedEmailsModel>
{
    public string? NotificationId { get; set; } = null!;
}

public class ProcessEmailsCommandHandler : IRequestHandler<ProcessEmailsCommand, ProcessedEmailsModel>
{
    private readonly INotificationsClientService _notificationsClientService;
    private readonly ILogger<ProcessEmailsCommandHandler> _logger;

    public ProcessEmailsCommandHandler(INotificationsClientService notificationsClientService,
        ILogger<ProcessEmailsCommandHandler> logger)
    {
        _notificationsClientService = notificationsClientService;
        _logger = logger;
    }

    public async Task<ProcessedEmailsModel> Handle(ProcessEmailsCommand request, CancellationToken cancellationToken)
    {
        var result = new ProcessedEmailsModel();

        _logger.LogInformation("Processing emails...");

        try
        {
            if (!string.IsNullOrWhiteSpace(request!.NotificationId))
            {
                var notificationOutcome = await _notificationsClientService.GetNotificationById(request!.NotificationId);
            }
        }
        catch { } //TODO - suppress for now while testing, prob let it throw normally?

        var notificationOutcomes = await _notificationsClientService.GetNotifications();

        /*
        Emails processing:
            Add emails to EmailLog table
                - Enquiry submitted:
                    - Enquirer email -> Status = NewToBeProcessed, ProcessFromDate = now
                    - TP email -> Status = WaitingToBeTriggered, ProcessFromDate = NULL -> add as linked to send on enquirer email NotifyDelivered
                    - Call processing for Enquirer email ref number straight away
                - TP responds:
                    - TP email -> Status = NewToBeProcessed, ProcessFromDate = now
                    - enquirer email -> Status = DelayedEmail, ProcessFromDate = next morning (TBC)
                        - Consider how this works for batching, may need to update existing email if there are multiples - so uses a single email for responses and different template?
                    - Call processing for TP email ref number straight away

        Process emails (every min from CURL or via code straight after added above - allow a ClientReferenceNumber to be passed in so can process a single email)
        Pre-sending processing:
            1) Update all currently in status that has flag PollForStatusUpdateIfSent = true and LastEmailSendAttemptDate <= now and FinishProcessingDate > now.  Only if status has changed do the following:
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



        return result;
    }
}
