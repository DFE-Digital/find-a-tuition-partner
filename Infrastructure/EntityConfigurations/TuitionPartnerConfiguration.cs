using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerConfiguration : IEntityTypeConfiguration<TuitionPartner>
{
    public void Configure(EntityTypeBuilder<TuitionPartner> builder)
    {
        builder.HasIndex(e => e.SeoUrl).IsUnique();
        builder.HasIndex(e => e.Name);
        builder.Property(u => u.HasLogo)
            .HasComputedColumnSql("case when \"Logo\" is null then false else true end", stored: true);
    }
}