using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class DataProtectionKeyConfigurations : IEntityTypeConfiguration<DataProtectionKey>
{
    public void Configure(EntityTypeBuilder<DataProtectionKey> builder)
    {
        builder.ToTable("DataProtectionKeys");
        builder.Property(x => x.Id).HasColumnName("Id");
        builder.Property(x => x.Xml).HasColumnName("Xml");
        builder.Property(x => x.FriendlyName).HasColumnName("FriendlyName");
    }
}