using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EnquiryResponseConfigurations : IEntityTypeConfiguration<EnquiryResponse>
{
    public void Configure(EntityTypeBuilder<EnquiryResponse> builder)
    {
        builder.HasOne(e => e.EnquirerResponseEmailLog).WithOne(e => e.EnquirerResponse).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.TuitionPartnerResponseEmailLog).WithOne(e => e.TuitionPartnerResponse).OnDelete(DeleteBehavior.Restrict);
    }
}