using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EmailTriggerActivationConfigurations : IEntityTypeConfiguration<EmailTriggerActivation>
{
    public void Configure(EntityTypeBuilder<EmailTriggerActivation> builder)
    {
        builder.HasIndex(e => new { e.EmailLogId, e.ActivateEmailLogId }).IsUnique();
    }
}