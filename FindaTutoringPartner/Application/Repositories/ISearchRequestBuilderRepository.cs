namespace Application.Repositories;

public interface ISearchRequestBuilderRepository
{
    Task<TuitionPartnerSearchRequestBuilder> CreateAsync();
    Task<TuitionPartnerSearchRequestBuilder> RetrieveAsync(Guid searchId);
}