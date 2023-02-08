using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TuitionPartnerEnquirySeoUrlConfigurations : IEntityTypeConfiguration<TuitionPartnerEnquirySeoUrl>
{
    public void Configure(EntityTypeBuilder<TuitionPartnerEnquirySeoUrl> builder)
    {
        builder.HasIndex(e => e.SeoUrl);
    }
}