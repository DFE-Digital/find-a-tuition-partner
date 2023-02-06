using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TuitionType = Domain.Enums.TuitionType;

namespace Infrastructure.EntityConfigurations;

public class TuitionTypeConfiguration : IEntityTypeConfiguration<Domain.TuitionType>
{
    public void Configure(EntityTypeBuilder<Domain.TuitionType> builder)
    {
        builder.HasIndex(e => e.SeoUrl).IsUnique();
        builder.HasIndex(e => e.Name);

        builder.HasData(
            new Domain.TuitionType { Id = (int)TuitionType.Online, SeoUrl = "online", Name = "Online" },
            new Domain.TuitionType { Id = (int)TuitionType.InSchool, SeoUrl = "in-school", Name = "In School" }
        );
    }
}