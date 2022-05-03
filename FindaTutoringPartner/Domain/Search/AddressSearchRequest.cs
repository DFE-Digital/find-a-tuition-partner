namespace Domain.Search;

public class AddressSearchRequest : SearchRequestBase
{
    public string Postcode { get; set; } = null!;
}