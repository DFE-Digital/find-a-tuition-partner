using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class LocalAuthorityConfiguration : IEntityTypeConfiguration<LocalAuthority>
    {
        public void Configure(EntityTypeBuilder<LocalAuthority> builder)
        {
            builder.HasIndex(e => e.Code);
        }
    }
}
