using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EstablishmentTypeGroupConfiguration : IEntityTypeConfiguration<EstablishmentTypeGroup>
{
    public void Configure(EntityTypeBuilder<EstablishmentTypeGroup> builder)
    {
        builder.HasIndex(e => e.Name);
        builder.HasData(
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.Colleges, Name = "Colleges"},
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.Universities, Name = "Universities"},
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.IndependentSchools, Name = "Independent schools"},
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.LocalAuthorityMaintainedSchools, Name = "Local authority maintained schools"},
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.SpecialSchools, Name = "Special schools"},
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.WelshSchools, Name = "Welsh schools"},
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.OtherTypes, Name = "Other types"},
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.Academies, Name = "Academies"},
            new EstablishmentTypeGroup { Id = (int)EstablishmentTypeGroups.FreeSchools, Name = "Free schools" }
        );
    }
}