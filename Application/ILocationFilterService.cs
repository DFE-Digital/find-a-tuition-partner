using Domain.Search;

namespace Application;

public interface ILocationFilterService
{
    Task<LocationFilterParameters?> GetLocationFilterParametersAsync(string postcode);
}