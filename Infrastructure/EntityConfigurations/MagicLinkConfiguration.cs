using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MagicLinkTypeEnum = Domain.Enums.MagicLinkType;

namespace Infrastructure.EntityConfigurations;

public class MagicLinkConfiguration : IEntityTypeConfiguration<MagicLink>
{
    public void Configure(EntityTypeBuilder<MagicLink> builder)
    {
        builder.HasOne(x => x.Enquiry)
            .WithMany(x => x.MagicLinks)
            .HasForeignKey("EnquiryId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}