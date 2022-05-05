using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerConfiguration : IEntityTypeConfiguration<TuitionPartner>
{
    public void Configure(EntityTypeBuilder<TuitionPartner> builder)
    {
        builder.HasIndex(e => e.Name);
    }
}