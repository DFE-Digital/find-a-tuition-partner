using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class KeyStageSubjectEnquiryConfigurations : IEntityTypeConfiguration<KeyStageSubjectEnquiry>
{
    public void Configure(EntityTypeBuilder<KeyStageSubjectEnquiry> builder)
    {
        builder.HasIndex(e => e.KeyStageId);

        builder.HasIndex(e => e.SubjectId);
    }
}