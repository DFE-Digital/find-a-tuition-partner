using Application.Exceptions;
using Domain.Constants;
using Domain.Search;

namespace Application.Extensions;

public static class LocationFilterParametersExtensions
{
    public static LocationFilterParameters? Validate(this LocationFilterParameters? parameters)
    {
        if (parameters == null)
        {
            throw new LocationNotFoundException();
        }

        if (parameters.Country != Country.Name.England)
        {
            throw new LocationNotAvailableException();
        }

        if (string.IsNullOrWhiteSpace(parameters.LocalAuthorityDistrictCode))
        {
            throw new LocationNotMappedException();
        }

        return parameters;
    }
}