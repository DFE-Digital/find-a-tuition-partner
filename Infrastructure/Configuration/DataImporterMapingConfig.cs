using Domain;
using Mapster;

namespace Infrastructure.Configuration
{
    public static class DataImporterMapingConfig
    {
        public static void Configure()
        {
            ConfigureLocalAuthorityDistrictCoverage();
            ConfigureSubjectCoverage();
            ConfigurePrice();
            ConfigureTuitionPartner();
        }

        private static void ConfigureLocalAuthorityDistrictCoverage()
        {
            TypeAdapterConfig<LocalAuthorityDistrictCoverage, LocalAuthorityDistrictCoverage>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.TuitionPartnerId)
                .Ignore(dest => dest.TuitionPartner!)
                .Ignore(dest => dest.TuitionType!)
                .Ignore(dest => dest.LocalAuthorityDistrict!);
        }

        private static void ConfigureSubjectCoverage()
        {
            TypeAdapterConfig<SubjectCoverage, SubjectCoverage>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.TuitionPartnerId)
                .Ignore(dest => dest.TuitionPartner!)
                .Ignore(dest => dest.TuitionType!)
                .Ignore(dest => dest.Subject!);
        }

        private static void ConfigurePrice()
        {
            TypeAdapterConfig<Price, Price>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.TuitionPartnerId)
                .Ignore(dest => dest.TuitionPartner!)
                .Ignore(dest => dest.TuitionType!)
                .Ignore(dest => dest.Subject!);
        }

        private static void ConfigureTuitionPartner()
        {
            TypeAdapterConfig<TuitionPartner, TuitionPartner>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Prices!)
                .Ignore(dest => dest.LocalAuthorityDistrictCoverage!)
                .Ignore(dest => dest.SubjectCoverage!)
                .Ignore(dest => dest.Logo!)
                .Ignore(dest => dest.HasLogo)
                .Ignore(dest => dest.ImportProcessLastUpdatedData)
                .Ignore(dest => dest.IsActive)
                .Ignore(dest => dest.OrganisationType);
        }
    }
}
