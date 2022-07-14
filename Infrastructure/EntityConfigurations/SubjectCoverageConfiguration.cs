using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class SubjectCoverageConfiguration : IEntityTypeConfiguration<SubjectCoverage>
{
    public void Configure(EntityTypeBuilder<SubjectCoverage> builder)
    {
        builder.HasIndex(e => new { e.TuitionPartnerId, e.TuitionTypeId, e.SubjectId });
        builder.HasIndex(e => new { e.TuitionPartnerId, e.SubjectId });
        builder.HasIndex(e => new { e.TuitionTypeId, e.SubjectId });
    }
}