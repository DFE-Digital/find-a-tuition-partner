using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class ScheduledProcessingInfoConfiguration : IEntityTypeConfiguration<ScheduledProcessingInfo>
{
    public void Configure(EntityTypeBuilder<ScheduledProcessingInfo> builder)
    {
        builder.HasIndex(e => e.ScheduleName).IsUnique();
    }
}