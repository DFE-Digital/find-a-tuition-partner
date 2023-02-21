using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MagicLinkTypeEnum = Domain.Enums.MagicLinkType;

namespace Infrastructure.EntityConfigurations;

public class MagicLinkTypeConfiguration : IEntityTypeConfiguration<MagicLinkType>
{
    public void Configure(EntityTypeBuilder<MagicLinkType> builder)
    {
        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasData(
            new MagicLinkType { Id = (int)MagicLinkTypeEnum.EnquiryRequest, Name = nameof(MagicLinkTypeEnum.EnquiryRequest) },
            new MagicLinkType { Id = (int)MagicLinkTypeEnum.EnquirerViewResponse, Name = nameof(MagicLinkTypeEnum.EnquirerViewResponse) },
            new MagicLinkType { Id = (int)MagicLinkTypeEnum.EnquirerViewAllResponses, Name = nameof(MagicLinkTypeEnum.EnquirerViewAllResponses) }
        );
    }
}