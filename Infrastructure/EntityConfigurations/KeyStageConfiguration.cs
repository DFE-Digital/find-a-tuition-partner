using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class KeyStageConfiguration : IEntityTypeConfiguration<KeyStage>
{
    public void Configure(EntityTypeBuilder<KeyStage> builder)
    {
        builder.HasData(
            new KeyStage { Id = KeyStages.Id.One, Name = "Key stage 1" },
            new KeyStage { Id = KeyStages.Id.Two, Name = "Key stage 2" },
            new KeyStage { Id = KeyStages.Id.Three, Name = "Key stage 3" },
            new KeyStage { Id = KeyStages.Id.Four, Name = "Key stage 4" }
        );
    }
}