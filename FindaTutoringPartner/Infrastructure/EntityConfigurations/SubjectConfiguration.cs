using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasData(
            new Subject {Id = Subjects.Id.PrimaryLiteracy, Name = "Primary - Literacy"},
            new Subject {Id = Subjects.Id.PrimaryNumeracy, Name = "Primary - Numeracy"},
            new Subject {Id = Subjects.Id.PrimaryScience, Name = "Primary - Science"},
            new Subject {Id = Subjects.Id.SecondaryEnglish, Name = "Secondary - English"},
            new Subject {Id = Subjects.Id.SecondaryHumanities, Name = "Secondary - Humanities"},
            new Subject {Id = Subjects.Id.SecondaryMaths, Name = "Secondary - Maths"},
            new Subject {Id = Subjects.Id.SecondaryModernForeignLanguages, Name = "Secondary - Modern Foreign Languages"},
            new Subject {Id = Subjects.Id.SecondaryScience, Name = "Secondary - Science"}
        );
    }
}