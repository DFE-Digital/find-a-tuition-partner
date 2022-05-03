namespace Domain.Search;

public class AddressSearchResultsPage : SearchResultsPage<AddressSearchRequest, Address>
{
    public AddressSearchResultsPage(AddressSearchRequest request, int count, Address[] results) : base(request, count, results)
    {
    }
}