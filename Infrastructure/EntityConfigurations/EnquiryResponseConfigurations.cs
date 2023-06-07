using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EnquiryResponseConfigurations : IEntityTypeConfiguration<EnquiryResponse>
{
    public void Configure(EntityTypeBuilder<EnquiryResponse> builder)
    {
        builder.HasOne(e => e.EnquirerResponseEmailLog).WithMany(e => e.EnquirerResponses).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.TuitionPartnerResponseEmailLog).WithMany(e => e.TuitionPartnerResponses).OnDelete(DeleteBehavior.Restrict);
    }
}