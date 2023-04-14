using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EmailNotifyLogConfigurations : IEntityTypeConfiguration<EmailNotifyLog>
{
    public void Configure(EntityTypeBuilder<EmailNotifyLog> builder)
    {
        builder.HasIndex(e => e.NotifyId).IsUnique();
    }
}