using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EmailPersonalisationLogConfigurations : IEntityTypeConfiguration<EmailPersonalisationLog>
{
    public void Configure(EntityTypeBuilder<EmailPersonalisationLog> builder)
    {
        builder.HasIndex(e => new { e.EmailLogId, e.Key }).IsUnique();
    }
}