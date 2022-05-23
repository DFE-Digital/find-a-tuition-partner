using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionTypeConfiguration : IEntityTypeConfiguration<TuitionType>
{
    public void Configure(EntityTypeBuilder<TuitionType> builder)
    {
        builder.HasData(
            new TuitionType { Id = 1, Name = "Online" },
            new TuitionType { Id = 2, Name = "In Person" }
        );
    }
}