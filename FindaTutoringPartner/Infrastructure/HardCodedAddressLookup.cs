using Application;
using Domain.Search;

namespace Infrastructure;

public class HardCodedAddressLookup : IAddressLookup
{
    public AddressSearchResultsPage LookupAddress(string postcode)
    {
        return LookupAddressAsync(postcode).Result;
    }

    public async Task<AddressSearchResultsPage> LookupAddressAsync(string postcode)
    {
        throw new NotImplementedException();
    }
}