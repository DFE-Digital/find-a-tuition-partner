using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TutorTypeConfiguration : IEntityTypeConfiguration<TutorType>
{
    public void Configure(EntityTypeBuilder<TutorType> builder)
    {
    }
}