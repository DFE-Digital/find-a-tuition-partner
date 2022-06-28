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
            new Subject {Id = Subjects.Id.PrimaryLiteracy, KeyStageId = KeyStages.Id.One, Name = "Primary - Literacy"},
            new Subject {Id = Subjects.Id.PrimaryNumeracy, KeyStageId = KeyStages.Id.One, Name = "Primary - Numeracy"},
            new Subject {Id = Subjects.Id.PrimaryScience, KeyStageId = KeyStages.Id.One, Name = "Primary - Science"},
            new Subject {Id = Subjects.Id.SecondaryEnglish, KeyStageId = KeyStages.Id.Three, Name = "Secondary - English"},
            new Subject {Id = Subjects.Id.SecondaryHumanities, KeyStageId = KeyStages.Id.Three, Name = "Secondary - Humanities"},
            new Subject {Id = Subjects.Id.SecondaryMaths, KeyStageId = KeyStages.Id.Three, Name = "Secondary - Maths"},
            new Subject {Id = Subjects.Id.SecondaryModernForeignLanguages, KeyStageId = KeyStages.Id.Three, Name = "Secondary - Modern Foreign Languages"},
            new Subject {Id = Subjects.Id.SecondaryScience, KeyStageId = KeyStages.Id.Three, Name = "Secondary - Science"},

            new Subject {Id = Subjects.Id.KeyStage1Literacy, KeyStageId = KeyStages.Id.One, Name = "Literacy"},
            new Subject {Id = Subjects.Id.KeyStage1Numeracy, KeyStageId = KeyStages.Id.One, Name = "Numeracy"},
            new Subject {Id = Subjects.Id.KeyStage1Science, KeyStageId = KeyStages.Id.One, Name = "Science"},
            new Subject {Id = Subjects.Id.KeyStage2Literacy, KeyStageId = KeyStages.Id.Two, Name = "Literacy"},
            new Subject {Id = Subjects.Id.KeyStage2Numeracy, KeyStageId = KeyStages.Id.Two, Name = "Numeracy"},
            new Subject {Id = Subjects.Id.KeyStage2Science, KeyStageId = KeyStages.Id.Two, Name = "Literacy"},
            new Subject {Id = Subjects.Id.KeyStage3English, KeyStageId = KeyStages.Id.Three, Name = "English"},
            new Subject {Id = Subjects.Id.KeyStage3Humanities, KeyStageId = KeyStages.Id.Three, Name = "Humanities"},
            new Subject {Id = Subjects.Id.KeyStage3Maths, KeyStageId = KeyStages.Id.Three, Name = "Maths"},
            new Subject {Id = Subjects.Id.KeyStage3ModernForeignLanguages, KeyStageId = KeyStages.Id.Three, Name = "Modern Foreign Languages"},
            new Subject {Id = Subjects.Id.KeyStage3Science, KeyStageId = KeyStages.Id.Three, Name = "Science"},
            new Subject {Id = Subjects.Id.KeyStage4English, KeyStageId = KeyStages.Id.Four, Name = "English"},
            new Subject {Id = Subjects.Id.KeyStage4Humanities, KeyStageId = KeyStages.Id.Four, Name = "Humanities"},
            new Subject {Id = Subjects.Id.KeyStage4Maths, KeyStageId = KeyStages.Id.Four, Name = "Maths"},
            new Subject {Id = Subjects.Id.KeyStage4ModernForeignLanguages, KeyStageId = KeyStages.Id.Four, Name = "Modern Foreign Languages"},
            new Subject {Id = Subjects.Id.KeyStage4Science, KeyStageId = KeyStages.Id.Four, Name = "Science"}
        );
    }
}