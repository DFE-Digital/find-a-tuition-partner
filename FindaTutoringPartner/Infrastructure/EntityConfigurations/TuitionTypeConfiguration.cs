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
            new TuitionType { Id = TuitionTypes.Id.Online, Name = "Online" },
            new TuitionType { Id = TuitionTypes.Id.InPerson, Name = "In Person" }
        );
    }
}