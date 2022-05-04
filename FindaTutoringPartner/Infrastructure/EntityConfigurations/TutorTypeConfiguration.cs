using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TutorTypeConfiguration : IEntityTypeConfiguration<TutorType>
{
    public void Configure(EntityTypeBuilder<TutorType> builder)
    {
        builder.HasData(
            new TutorType { Id = 1, Name = "Qualified Teachers" },
            new TutorType { Id = 2, Name = "Professional Tutors" },
            new TutorType { Id = 3, Name = "SEN Specialists" },
            new TutorType { Id = 4, Name = "Higher Level Teaching Assistants" },
            new TutorType { Id = 5, Name = "University Students" },
            new TutorType { Id = 6, Name = "Volunteer tutors" },
            new TutorType { Id = 7, Name = "No preference" }
        );
    }
}