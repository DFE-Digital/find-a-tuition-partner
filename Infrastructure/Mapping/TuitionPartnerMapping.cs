using Domain.Search;

namespace Infrastructure.Mapping;

using Mapster;

public static class TuitionPartnerMapping
{
    public static void Configure()
    {
        ConfigureTuitionPartner();
    }
    private static void ConfigureTuitionPartner()
    {
        TypeAdapterConfig<Domain.TuitionPartner, TuitionPartnerResult>
            .NewConfig()
            //Mapster has issues mapping Prices, due to circular ref, but ignore it anyway, since done within code when needed
            .Ignore(dest => dest.Prices!);
    }
}