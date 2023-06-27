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

        builder.HasIndex(e => e.ClientReferenceNumber);

        builder.HasOne(e => e.EmailStatus).WithMany(e => e.EmailLogs).OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new EmailLog
            {
                Id = 1,
                CreatedDate = new DateTime(2023, 6, 7, 20, 55, 43, 728, DateTimeKind.Utc).AddTicks(2661),
                FinishProcessingDate = new DateTime(2023, 6, 7, 20, 55, 43, 728, DateTimeKind.Utc).AddTicks(2662),
                EmailAddress = "historical_emails_when_log_implemented",
                EmailTemplateShortName = "historical_emails_when_log_implemented",
                ClientReferenceNumber = "historical_emails_when_log_implemented",
                EmailStatusId = (int)EmailStatus.NotifyDelivered
            }
        );
    }
}