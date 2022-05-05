using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasData(
            new Subject {Id = 1, Name = "Primary - Literacy"},
            new Subject {Id = 2, Name = "Primary - Numeracy"},
            new Subject {Id = 3, Name = "Primary - Science"},
            new Subject {Id = 4, Name = "Secondary - English"},
            new Subject {Id = 5, Name = "Secondary - Humanities"},
            new Subject {Id = 6, Name = "Secondary - Maths"},
            new Subject {Id = 7, Name = "Secondary - Modern Foreign Languages"},
            new Subject {Id = 8, Name = "Secondary - Science"}
            );
    }
}