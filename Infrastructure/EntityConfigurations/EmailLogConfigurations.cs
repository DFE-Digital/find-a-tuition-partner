using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EmailStatus = Domain.Enums.EmailStatus;

namespace Infrastructure.EntityConfigurations;

public class EmailLogConfigurations : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.HasIndex(e => e.ProcessFromDate);

        builder.HasIndex(e => e.LastEmailSendAttemptDate);

        builder.HasIndex(e => e.FinishProcessingDate);

        builder.HasIndex(e => e.ClientReferenceNumber).IsUnique();

        builder.HasOne(e => e.EmailStatus).WithMany(e => e.EmailLogs).OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new EmailLog
            { //TODO - confirm this is the best way to deal with historical data where we don't have the email details
                Id = 1,
                CreatedDate = DateTime.UtcNow,
                FinishProcessingDate = DateTime.UtcNow,
                EmailAddress = "historical_emails_when_log_implemented",
                EmailTemplateShortName = "historical_emails_when_log_implemented",
                ClientReferenceNumber = "historical_emails_when_log_implemented",
                EmailStatusId = (int)EmailStatus.NotifyDelivered
            }
        );
    }
}