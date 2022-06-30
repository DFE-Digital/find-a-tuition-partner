using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionTypeConfiguration : IEntityTypeConfiguration<TuitionType>
{
    public void Configure(EntityTypeBuilder<TuitionType> builder)
    {
        builder.HasData(
            new TuitionType { Id = (int)TuitionTypes.Online, Name = "Online" },
            new TuitionType { Id = (int)TuitionTypes.InPerson, Name = "In Person" }
        );
    }
}