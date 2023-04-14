using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EmailLogConfigurations : IEntityTypeConfiguration<EmailLog>
{
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.HasIndex(e => e.ProcessFromDate);

        builder.HasIndex(e => e.LastEmailSendAttemptDate);

        builder.HasIndex(e => e.FinishProcessingDate);

        builder.HasIndex(e => e.ClientReferenceNumber).IsUnique();
    }
}