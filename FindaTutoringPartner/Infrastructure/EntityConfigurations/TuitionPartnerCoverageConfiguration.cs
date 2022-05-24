using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerCoverageConfiguration : IEntityTypeConfiguration<TuitionPartnerCoverage>
{
    public void Configure(EntityTypeBuilder<TuitionPartnerCoverage> builder)
    {
        builder.HasIndex(e => new { e.TuitionPartnerId, e.LocalAuthorityDistrictId, e.TuitionTypeId })
            .IsUnique();
        builder.HasIndex(e => new { e.LocalAuthorityDistrictId, e.TuitionTypeId });
    }
}