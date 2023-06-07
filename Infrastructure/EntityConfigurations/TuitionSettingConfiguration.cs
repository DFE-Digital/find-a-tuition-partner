using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace Infrastructure.EntityConfigurations;

public class TuitionSettingConfiguration : IEntityTypeConfiguration<Domain.TuitionSetting>
{
    public void Configure(EntityTypeBuilder<Domain.TuitionSetting> builder)
    {
        builder.HasIndex(e => e.SeoUrl).IsUnique();
        builder.HasIndex(e => e.Name);

        builder.HasData(
            new Domain.TuitionSetting { Id = (int)TuitionSetting.Online, SeoUrl = "online", Name = "Online" },
            new Domain.TuitionSetting { Id = (int)TuitionSetting.FaceToFace, SeoUrl = "face-to-face", Name = "Face-to-face" }
        );
    }
}