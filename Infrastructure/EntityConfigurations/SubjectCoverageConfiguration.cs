using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class SubjectCoverageConfiguration : IEntityTypeConfiguration<SubjectCoverage>
{
    public void Configure(EntityTypeBuilder<SubjectCoverage> builder)
    {
        builder.HasIndex(e => new { e.TuitionPartnerId, e.TuitionSettingId, e.SubjectId });
        builder.HasIndex(e => new { e.TuitionPartnerId, e.SubjectId });
        builder.HasIndex(e => new { e.TuitionSettingId, e.SubjectId });
    }
}