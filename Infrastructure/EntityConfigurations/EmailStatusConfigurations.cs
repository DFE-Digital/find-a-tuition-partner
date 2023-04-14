using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EmailStatusConfigurations : IEntityTypeConfiguration<EmailStatus>
{
    public void Configure(EntityTypeBuilder<EmailStatus> builder)
    {
        builder.HasIndex(e => e.Status).IsUnique();
    }
}