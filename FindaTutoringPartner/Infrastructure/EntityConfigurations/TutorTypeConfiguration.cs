using Domain;
using Domain.Constants;
using Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TutorTypeConfiguration : IEntityTypeConfiguration<TutorType>
{
    public void Configure(EntityTypeBuilder<TutorType> builder)
    {
        builder.HasData(
            new TutorType { Id = TutorTypes.Id.QualifiedTeachers, Name = "Qualified Teachers" },
            new TutorType { Id = TutorTypes.Id.ProfessionalTutors, Name = "Professional Tutors" },
            new TutorType { Id = TutorTypes.Id.SenSpecialists, Name = "SEN Specialists" },
            new TutorType { Id = TutorTypes.Id.HigherLevelTeachingAssistants, Name = "Higher Level Teaching Assistants" },
            new TutorType { Id = TutorTypes.Id.UniversityStudents, Name = "University Students" },
            new TutorType { Id = TutorTypes.Id.VolunteerTutors, Name = "Volunteer tutors" },
            new TutorType { Id = TutorTypes.Id.NoPreference, Name = "No preference" }
        );
    }
}