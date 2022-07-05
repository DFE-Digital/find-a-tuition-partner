using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class KeyStageConfiguration : IEntityTypeConfiguration<KeyStage>
{
    public void Configure(EntityTypeBuilder<KeyStage> builder)
    {
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.SeoUrl).IsUnique();

        builder.HasData(
            new KeyStage { Id = KeyStages.Id.One, SeoUrl = "key-stage-1", Name = "Key stage 1" },
            new KeyStage { Id = KeyStages.Id.Two, SeoUrl = "key-stage-2", Name = "Key stage 2" },
            new KeyStage { Id = KeyStages.Id.Three, SeoUrl = "key-stage-3", Name = "Key stage 3" },
            new KeyStage { Id = KeyStages.Id.Four, SeoUrl = "key-stage-4", Name = "Key stage 4" }
        );
    }
}