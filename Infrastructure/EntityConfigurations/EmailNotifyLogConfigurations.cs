using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EmailNotifyLogConfigurations : IEntityTypeConfiguration<EmailNotifyResponseLog>
{
    public void Configure(EntityTypeBuilder<EmailNotifyResponseLog> builder)
    {
        builder.HasIndex(e => e.NotifyId).IsUnique();
    }
}