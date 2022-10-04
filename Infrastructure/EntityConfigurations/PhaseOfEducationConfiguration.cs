using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class PhaseOfEducationConfiguration : IEntityTypeConfiguration<PhaseOfEducation>
{
    public void Configure(EntityTypeBuilder<PhaseOfEducation> builder)
    {
        builder.HasIndex(e => e.Name);

        builder.HasData(
            new PhaseOfEducation { Id = (int)PhasesOfEducation.Nursery, Name = "Nursery" },
            new PhaseOfEducation { Id = (int)PhasesOfEducation.Primary, Name = "Primary" },
            new PhaseOfEducation { Id = (int)PhasesOfEducation.MiddleDeemedPrimary, Name = "Middle deemed primary" },
            new PhaseOfEducation { Id = (int)PhasesOfEducation.Secondary, Name = "Secondary" },
            new PhaseOfEducation { Id = (int)PhasesOfEducation.MiddleDeemedSecondary, Name = "Middle deemed secondary" },
            new PhaseOfEducation { Id = (int)PhasesOfEducation.SixteenPlus, Name = "16 Plus" },
            new PhaseOfEducation { Id = (int)PhasesOfEducation.AllThrough, Name = "All-through" }
        );
    }
}