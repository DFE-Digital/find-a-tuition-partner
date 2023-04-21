using Application.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EmailStatus = Domain.Enums.EmailStatus;

namespace Infrastructure.EntityConfigurations;

public class EmailStatusConfigurations : IEntityTypeConfiguration<Domain.EmailStatus>
{
    public void Configure(EntityTypeBuilder<Domain.EmailStatus> builder)
    {
        builder.HasIndex(e => e.Status).IsUnique();

        builder.HasData(
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.ToBeProcessed,
                Status = EmailStatus.ToBeProcessed.DisplayName(),
                Description = "Has been newly added to log, will be processed next time the email processing is run",
                AllowEmailSending = true,
                PollForStatusUpdateIfSent = true,
                RetrySendInSeconds = null
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.WaitingToBeTriggered,
                Status = EmailStatus.WaitingToBeTriggered.DisplayName(),
                Description = "Is waiting for a chained email to be delivered (e.g. TP emails are only sent once the enquirer email has been delivered)",
                AllowEmailSending = true,
                PollForStatusUpdateIfSent = true,
                RetrySendInSeconds = null
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.DelayedEmail,
                Status = EmailStatus.DelayedEmail.DisplayName(),
                Description = "The email is to be sent is to be sent in the future (e.g. send notification emails daily)",
                AllowEmailSending = true,
                PollForStatusUpdateIfSent = true,
                RetrySendInSeconds = null
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.BeenProcessed,
                Status = EmailStatus.BeenProcessed.DisplayName(),
                Description = "The email has been processed and sent to GOV.UK Notify to be delivered",
                AllowEmailSending = false,
                PollForStatusUpdateIfSent = true,
                RetrySendInSeconds = null
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.NotifyCreated,
                Status = EmailStatus.NotifyCreated.DisplayName(),
                Description = "GOV.UK Notify status: has placed the message in a queue, ready to be sent to the provider. It should only remain in this state for a few seconds.",
                AllowEmailSending = false,
                PollForStatusUpdateIfSent = true,
                RetrySendInSeconds = null
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.NotifySending,
                Status = EmailStatus.NotifySending.DisplayName(),
                Description = "GOV.UK Notify status: has sent the message to the provider. The provider will try to deliver the message to the recipient for up to 72 hours. GOV.UK Notify is waiting for delivery information.",
                AllowEmailSending = false,
                PollForStatusUpdateIfSent = true,
                RetrySendInSeconds = null
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.NotifyDelivered,
                Status = EmailStatus.NotifyDelivered.DisplayName(),
                Description = "GOV.UK Notify status: the message was successfully delivered.",
                AllowEmailSending = false,
                PollForStatusUpdateIfSent = false,
                RetrySendInSeconds = null
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.NotifyPermanentFailure,
                Status = EmailStatus.NotifyPermanentFailure.DisplayName(),
                Description = "GOV.UK Notify status: the provider could not deliver the message because the email address was wrong. You should remove these email addresses from your database.",
                AllowEmailSending = false,
                PollForStatusUpdateIfSent = false,
                RetrySendInSeconds = null
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.NotifyTemporaryFailure,
                Status = EmailStatus.NotifyTemporaryFailure.DisplayName(),
                Description = "GOV.UK Notify status: the provider could not deliver the message. This can happen when the recipient’s inbox is full or their anti-spam filter rejects your email. Check your content does not look like spam before you try to send the message again.",
                AllowEmailSending = true,
                PollForStatusUpdateIfSent = true,
                RetrySendInSeconds = 60 * 10
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.NotifyTechnicalFailure,
                Status = EmailStatus.NotifyTechnicalFailure.DisplayName(),
                Description = "GOV.UK Notify status: your message was not sent because there was a problem between Notify and the provider. You’ll have to try sending your messages again.",
                AllowEmailSending = true,
                PollForStatusUpdateIfSent = true,
                RetrySendInSeconds = 60 * 1
            },
            new Domain.EmailStatus
            {
                Id = (int)EmailStatus.ProcessingFailure,
                Status = EmailStatus.ProcessingFailure.DisplayName(),
                Description = "Error when calling the GOV.UK Notify SendEmailAsync()",
                AllowEmailSending = true,
                PollForStatusUpdateIfSent = false,
                RetrySendInSeconds = 60 * 10
            }
        );
    }
}