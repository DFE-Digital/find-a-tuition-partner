using Application;
using Domain;
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
        var request = new AddressSearchRequest
        {
            Postcode = postcode
        };

        var results = new Address[]
        {
            new Address {Line1 = "10 Test Road", Line3 = "Testington", Postcode = postcode},
            new Address {Line1 = "11 Test Road", Line3 = "Testington", Postcode = postcode},
            new Address {Line1 = "12 Test Road", Line3 = "Testington", Postcode = postcode},
            new Address {Line1 = "13 Test Road", Line3 = "Testington", Postcode = postcode},
            new Address {Line1 = "14 Test Road", Line3 = "Testington", Postcode = postcode},
        };

        return new AddressSearchResultsPage(request, results.Length, results);
    }
}