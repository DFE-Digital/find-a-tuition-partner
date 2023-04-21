using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EmailLogHistoryConfigurations : IEntityTypeConfiguration<EmailLogHistory>
{
    public void Configure(EntityTypeBuilder<EmailLogHistory> builder)
    {
    }
}