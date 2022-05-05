using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerLocationConfiguration : IEntityTypeConfiguration<TuitionPartnerLocation>
{
    public void Configure(EntityTypeBuilder<TuitionPartnerLocation> builder)
    {
    }
}