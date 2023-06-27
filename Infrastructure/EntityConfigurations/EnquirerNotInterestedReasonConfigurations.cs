using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EnquirerNotInterestedReasonConfigurations : IEntityTypeConfiguration<Domain.EnquirerNotInterestedReason>
{
    public void Configure(EntityTypeBuilder<Domain.EnquirerNotInterestedReason> builder)
    {
        builder.HasMany(e => e.EnquiryResponses).WithOne(e => e.EnquirerNotInterestedReason).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Description).IsUnique();

        builder.HasData(
            new Domain.EnquirerNotInterestedReason
            {
                Id = 1,
                Description = "The response does not adequately cover my tuition plan needs",
                CollectAdditionalInfoIfSelected = false,
                OrderBy = 1,
                IsActive = true
            },
            new Domain.EnquirerNotInterestedReason
            {
                Id = 2,
                Description = "The response does not adequately cover support for our pupils with SEND",
                CollectAdditionalInfoIfSelected = false,
                OrderBy = 2,
                IsActive = true
            },
            new Domain.EnquirerNotInterestedReason
            {
                Id = 3,
                Description = "The response is too generic and doesn’t offer enough information",
                CollectAdditionalInfoIfSelected = false,
                OrderBy = 3,
                IsActive = true
            },
            new Domain.EnquirerNotInterestedReason
            {
                Id = 4,
                Description = "Other",
                CollectAdditionalInfoIfSelected = true,
                OrderBy = 4,
                IsActive = true
            }
        );
    }
}