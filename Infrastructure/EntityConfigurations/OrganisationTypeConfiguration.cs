using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class OrganisationTypeConfiguration : IEntityTypeConfiguration<OrganisationType>
{
    public void Configure(EntityTypeBuilder<OrganisationType> builder)
    {
        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.IsTypeOfCharity);

        builder.HasData(
            new OrganisationType { Id = (int)OrganisationTypes.PrivateCompany, Name = "Private company", IsTypeOfCharity = false },
            new OrganisationType { Id = (int)OrganisationTypes.LimitedCompany, Name = "Limited company", IsTypeOfCharity = false },
            new OrganisationType { Id = (int)OrganisationTypes.LimitedLiabilityPartnership, Name = "Limited liability partnership", IsTypeOfCharity = false },
            new OrganisationType { Id = (int)OrganisationTypes.PrivateCompanyLimitedByGuarantee, Name = "Private company limited by guarantee", IsTypeOfCharity = true },
            new OrganisationType { Id = (int)OrganisationTypes.Charity, Name = "Charity/charities", IsTypeOfCharity = true },
            new OrganisationType { Id = (int)OrganisationTypes.NonProfit, Name = "Non-profit", IsTypeOfCharity = true },
            new OrganisationType { Id = (int)OrganisationTypes.CommunityInterestCompany, Name = "Community interest company", IsTypeOfCharity = true }
        );
    }
}