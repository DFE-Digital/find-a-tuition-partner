using Application.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnquiryResponseStatus = Domain.Enums.EnquiryResponseStatus;

namespace Infrastructure.EntityConfigurations;

public class EnquiryResponseStatusConfigurations : IEntityTypeConfiguration<Domain.EnquiryResponseStatus>
{
    public void Configure(EntityTypeBuilder<Domain.EnquiryResponseStatus> builder)
    {
        builder.HasIndex(e => e.Status).IsUnique();

        builder.HasData(
            new Domain.EnquiryResponseStatus
            {
                Id = (int)EnquiryResponseStatus.Interested,
                Status = EnquiryResponseStatus.Interested.DisplayName(),
                Description = "The enquirer has indicated that they are interested in the tuition partner response",
                OrderBy = 1
            },
            new Domain.EnquiryResponseStatus
            {
                Id = (int)EnquiryResponseStatus.Undecided,
                Status = EnquiryResponseStatus.Undecided.DisplayName(),
                Description = "The enquirer has opened the tuition partner response, but has not confirmed if they are interested or not",
                OrderBy = 2
            },
            new Domain.EnquiryResponseStatus
            {
                Id = (int)EnquiryResponseStatus.Unread,
                Status = EnquiryResponseStatus.Unread.DisplayName(),
                Description = "The enquirer has not yet viewed the tuition partner response",
                OrderBy = 3
            },
            new Domain.EnquiryResponseStatus
            {
                Id = (int)EnquiryResponseStatus.NotSet,
                Status = EnquiryResponseStatus.NotSet.DisplayName(),
                Description = "Status that is used for enquries that are historical and we don't have the latest status for",
                OrderBy = 4
            },
            new Domain.EnquiryResponseStatus
            {
                Id = (int)EnquiryResponseStatus.Rejected,
                Status = EnquiryResponseStatus.Rejected.DisplayName(),
                Description = "The enquirer has indicated that they are not interested in the tuition partner response",
                OrderBy = 5
            }
        );
    }
}