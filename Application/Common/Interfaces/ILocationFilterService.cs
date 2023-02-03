using Domain.Search;

namespace Application.Common.Interfaces;

public interface ILocationFilterService
{
    Task<LocationFilterParameters?> GetLocationFilterParametersAsync(string postcode);
}