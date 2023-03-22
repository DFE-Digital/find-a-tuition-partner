using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EnquiryConfigurations : IEntityTypeConfiguration<Enquiry>
{
    public void Configure(EntityTypeBuilder<Enquiry> builder)
    {
        builder.HasIndex(e => e.Email);

        builder.HasIndex(e => e.SupportReferenceNumber).IsUnique();

        builder.HasMany(e => e.MagicLinks)
            .WithOne()
            .HasForeignKey("EnquiryId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}