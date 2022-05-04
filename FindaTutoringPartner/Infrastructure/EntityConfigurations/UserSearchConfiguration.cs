using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class UserSearchConfiguration : IEntityTypeConfiguration<UserSearch>
{
    public void Configure(EntityTypeBuilder<UserSearch> builder)
    {
        builder
            .Property(e => e.SearchJson)
            .HasColumnType("jsonb");
    }
}