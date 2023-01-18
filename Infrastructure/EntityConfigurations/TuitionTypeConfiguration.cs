using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionTypeConfiguration : IEntityTypeConfiguration<Domain.TuitionType>
{
    public void Configure(EntityTypeBuilder<Domain.TuitionType> builder)
    {
        builder.HasIndex(e => e.SeoUrl).IsUnique();
        builder.HasIndex(e => e.Name);

        builder.HasData(
            new Domain.TuitionType { Id = (int)Domain.Enums.TuitionType.Online, SeoUrl = "online", Name = "Online" },
            new Domain.TuitionType { Id = (int)Domain.Enums.TuitionType.InSchool, SeoUrl = "in-school", Name = "In School" }
        );
    }
}