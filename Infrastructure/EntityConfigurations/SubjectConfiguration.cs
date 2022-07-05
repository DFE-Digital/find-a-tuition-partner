using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.SeoUrl).IsUnique();

        builder.HasData(
            new Subject {Id = Subjects.Id.KeyStage1Literacy, SeoUrl = "key-stage-1-literacy", KeyStageId = KeyStages.Id.One, Name = "Literacy"},
            new Subject {Id = Subjects.Id.KeyStage1Numeracy, SeoUrl = "key-stage-1-numeracy", KeyStageId = KeyStages.Id.One, Name = "Numeracy"},
            new Subject {Id = Subjects.Id.KeyStage1Science, SeoUrl = "key-stage-1-science", KeyStageId = KeyStages.Id.One, Name = "Science"},
            new Subject {Id = Subjects.Id.KeyStage2Literacy, SeoUrl = "key-stage-2-literacy", KeyStageId = KeyStages.Id.Two, Name = "Literacy"},
            new Subject {Id = Subjects.Id.KeyStage2Numeracy, SeoUrl = "key-stage-2-numeracy", KeyStageId = KeyStages.Id.Two, Name = "Numeracy"},
            new Subject {Id = Subjects.Id.KeyStage2Science, SeoUrl = "key-stage-2-science", KeyStageId = KeyStages.Id.Two, Name = "Literacy"},
            new Subject {Id = Subjects.Id.KeyStage3English, SeoUrl = "key-stage-3-english", KeyStageId = KeyStages.Id.Three, Name = "English"},
            new Subject {Id = Subjects.Id.KeyStage3Humanities, SeoUrl = "key-stage-3-humanities", KeyStageId = KeyStages.Id.Three, Name = "Humanities"},
            new Subject {Id = Subjects.Id.KeyStage3Maths, SeoUrl = "key-stage-3-maths", KeyStageId = KeyStages.Id.Three, Name = "Maths"},
            new Subject {Id = Subjects.Id.KeyStage3ModernForeignLanguages, SeoUrl = "key-stage-3-modern-foreign-languages", KeyStageId = KeyStages.Id.Three, Name = "Modern Foreign Languages"},
            new Subject {Id = Subjects.Id.KeyStage3Science, SeoUrl = "key-stage-3-science", KeyStageId = KeyStages.Id.Three, Name = "Science"},
            new Subject {Id = Subjects.Id.KeyStage4English, SeoUrl = "key-stage-4-english", KeyStageId = KeyStages.Id.Four, Name = "English"},
            new Subject {Id = Subjects.Id.KeyStage4Humanities, SeoUrl = "key-stage-4-humanities", KeyStageId = KeyStages.Id.Four, Name = "Humanities"},
            new Subject {Id = Subjects.Id.KeyStage4Maths, SeoUrl = "key-stage-4-maths", KeyStageId = KeyStages.Id.Four, Name = "Maths"},
            new Subject {Id = Subjects.Id.KeyStage4ModernForeignLanguages, SeoUrl = "key-stage-4-modern-foreign-languages", KeyStageId = KeyStages.Id.Four, Name = "Modern Foreign Languages"},
            new Subject {Id = Subjects.Id.KeyStage4Science, SeoUrl = "key-stage-4-science", KeyStageId = KeyStages.Id.Four, Name = "Science"}
        );
    }
}