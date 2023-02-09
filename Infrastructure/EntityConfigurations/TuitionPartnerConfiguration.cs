using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerConfiguration : IEntityTypeConfiguration<TuitionPartner>, IEntityTypeConfiguration<TuitionPartnerLogo>
{
    public void Configure(EntityTypeBuilder<TuitionPartner> builder)
    {
        builder.ToTable("TuitionPartners");
        builder.HasIndex(e => e.SeoUrl).IsUnique();
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.ImportId).IsUnique();
        builder.HasIndex(e => e.IsActive);
        builder.Property(s => s.SeoUrl).HasColumnName("SeoUrl");
        builder.Property(u => u.HasLogo)
            .HasComputedColumnSql("case when \"Logo\" is null then false else true end", stored: true);

        builder.HasOne(s => s.Logo).WithOne().HasForeignKey<TuitionPartnerLogo>(x => x.Id);
    }
    public void Configure(EntityTypeBuilder<TuitionPartnerLogo> builder)
    {
        builder.ToTable("TuitionPartners");
    }
}
