using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerEnquiryConfigurations : IEntityTypeConfiguration<TuitionPartnerEnquiry>
{
    public void Configure(EntityTypeBuilder<TuitionPartnerEnquiry> builder)
    {
        builder.HasIndex(e => e.TuitionPartnerId);
    }
}