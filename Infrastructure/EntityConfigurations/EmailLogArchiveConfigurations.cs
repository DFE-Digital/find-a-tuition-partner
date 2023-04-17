using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EmailLogArchiveConfigurations : IEntityTypeConfiguration<EmailLogHistory>
{
    public void Configure(EntityTypeBuilder<EmailLogHistory> builder)
    {
        //builder.HasIndex(e => e.Email);

        //builder.HasIndex(e => e.SupportReferenceNumber).IsUnique();
    }
}