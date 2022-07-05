using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionTypeConfiguration : IEntityTypeConfiguration<TuitionType>
{
    public void Configure(EntityTypeBuilder<TuitionType> builder)
    {
        builder.HasIndex(e => e.SeoUrl).IsUnique();
        builder.HasIndex(e => e.Name);

        builder.HasData(
            new TuitionType { Id = (int)TuitionTypes.Online, SeoUrl = "online", Name = "Online" },
            new TuitionType { Id = (int)TuitionTypes.InSchool, SeoUrl = "in-school", Name = "In School" }
        );
    }
}