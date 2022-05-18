namespace Application;

public interface ISearchRequestBuilderRepository
{
    Task<TuitionPartnerSearchRequestBuilder> CreateAsync();
    Task<TuitionPartnerSearchRequestBuilder> RetrieveAsync(Guid searchId);
    Task<TuitionPartnerSearchRequestBuilder> UpdateAsync(TuitionPartnerSearchRequestBuilder builder);
    Task DeleteAsync(Guid searchId);
}