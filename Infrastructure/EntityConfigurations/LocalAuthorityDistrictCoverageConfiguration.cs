using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class LocalAuthorityDistrictCoverageConfiguration : IEntityTypeConfiguration<LocalAuthorityDistrictCoverage>
{
    public void Configure(EntityTypeBuilder<LocalAuthorityDistrictCoverage> builder)
    {
        builder.HasIndex(e => new { e.TuitionPartnerId, e.TuitionSettingId, e.LocalAuthorityDistrictId });
        builder.HasIndex(e => new { e.TuitionPartnerId, e.LocalAuthorityDistrictId });
        builder.HasIndex(e => new { e.TuitionSettingId, e.LocalAuthorityDistrictId });
    }
}