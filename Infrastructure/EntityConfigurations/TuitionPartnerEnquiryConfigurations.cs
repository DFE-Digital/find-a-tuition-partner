using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerEnquiryConfigurations : IEntityTypeConfiguration<TuitionPartnerEnquiry>
{
    public void Configure(EntityTypeBuilder<TuitionPartnerEnquiry> builder)
    {
        builder.HasIndex(e => e.TuitionPartnerId);

        builder.HasOne(e => e.TuitionPartnerEnquirySubmittedEmailLog).WithMany(e => e.TuitionPartnerEnquiriesSubmitted).OnDelete(DeleteBehavior.Restrict);
    }
}