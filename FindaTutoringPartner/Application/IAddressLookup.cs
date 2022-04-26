using Domain.Search;

namespace Application;

public interface IAddressLookup
{
    AddressSearchResultsPage LookupAddress(string postcode);
    Task<AddressSearchResultsPage> LookupAddressAsync(string postcode);
}