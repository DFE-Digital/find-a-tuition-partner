using Application.Exceptions;
using Application.Repositories;
using Domain.Search;

namespace Application;

public class TuitionPartnerSearchRequestBuilder
{
    private readonly ISearchStateRepository _searchStateRepository;
    private readonly ILocationFilterService _locationFilterService;
    private readonly ILookupDataRepository _lookupDataRepository;

    public TuitionPartnerSearchRequestBuilder(SearchState searchState, ISearchStateRepository searchStateRepository, ILocationFilterService locationFilterService, ILookupDataRepository lookupDataRepository)
    {
        SearchState = searchState;
        _searchStateRepository = searchStateRepository;
        _locationFilterService = locationFilterService;
        _lookupDataRepository = lookupDataRepository;
    }

    public Guid SearchId => SearchState.SearchId;
    public SearchState SearchState { get; private set; }

    public async Task<TuitionPartnerSearchRequestBuilder> WithPostcode(string? postcode)
    {
        if (postcode == SearchState.LocationFilterParameters?.Postcode) return this;

        if (postcode == null)
        {
            SearchState.LocationFilterParameters = null;
            return this;
        }

        var parameters = await _locationFilterService.GetLocationFilterParametersAsync(postcode);
        if (parameters == null)
        {
            throw new LocationNotFoundException();
        }

        SearchState.LocationFilterParameters = parameters;
        SearchState = await _searchStateRepository.UpdateAsync(SearchState);

        return this;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> WithSubjectIds(ICollection<int> subjectIds)
    {
        if (SearchState.Subjects != null && subjectIds.Count == SearchState.Subjects.Count)
        {
            var changed = false;
            foreach (var subjectId in subjectIds)
            {
                if (!SearchState.Subjects.ContainsKey(subjectId))
                {
                    changed = true;
                    break;
                }
            }

            if (!changed) return this;
        }

        var subjects = await _lookupDataRepository.GetSubjectsAsync();

        var subjectDictionary = new Dictionary<int, string>();

        foreach (var subject in subjects)
        {
            if (subjectIds.Contains(subject.Id))
            {
                subjectDictionary[subject.Id] = subject.Name;
            }
        }

        SearchState.Subjects = subjectDictionary;
        SearchState = await _searchStateRepository.UpdateAsync(SearchState);

        return this;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> WithTutorTypeIds(ICollection<int> tutorTypeIds)
    {
        if (SearchState.TutorTypes != null && tutorTypeIds.Count == SearchState.TutorTypes.Count)
        {
            var changed = false;
            foreach (var tutorTypeId in tutorTypeIds)
            {
                if (!SearchState.TutorTypes.ContainsKey(tutorTypeId))
                {
                    changed = true;
                    break;
                }
            }

            if (!changed) return this;
        }

        var tutorTypes = await _lookupDataRepository.GetTutorTypesAsync();

        var tutorTypeDictionary = new Dictionary<int, string>();

        foreach (var tutorType in tutorTypes)
        {
            if (tutorTypeIds.Contains(tutorType.Id))
            {
                tutorTypeDictionary[tutorType.Id] = tutorType.Name;
            }
        }

        SearchState.TutorTypes = tutorTypeDictionary;
        SearchState = await _searchStateRepository.UpdateAsync(SearchState);

        return this;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> WithTuitionTypeIds(ICollection<int> tuitionTypeIds)
    {
        if (SearchState.TuitionTypes != null && tuitionTypeIds.Count == SearchState.TuitionTypes.Count)
        {
            var changed = false;
            foreach (var tuitionTypeId in tuitionTypeIds)
            {
                if (!SearchState.TuitionTypes.ContainsKey(tuitionTypeId))
                {
                    changed = true;
                    break;
                }
            }

            if (!changed) return this;
        }

        var tuitionTypes = await _lookupDataRepository.GetTuitionTypesAsync();

        var tuitionTypeDictionary = new Dictionary<int, string>();

        foreach (var tuitionType in tuitionTypes)
        {
            if (tuitionTypeIds.Contains(tuitionType.Id))
            {
                tuitionTypeDictionary[tuitionType.Id] = tuitionType.Name;
            }
        }

        SearchState.TuitionTypes = tuitionTypeDictionary;
        SearchState = await _searchStateRepository.UpdateAsync(SearchState);

        return this;
    }

    public TuitionPartnerSearchRequest Build()
    {
        var request = new TuitionPartnerSearchRequest
        {
            LocalAuthorityDistrictCode = SearchState.LocationFilterParameters?.LocalAuthorityDistrictCode,
            SubjectIds = SearchState.Subjects?.Keys,
            TuitionTypeIds = SearchState.TuitionTypes?.Keys,
            PageSize = SearchRequestBase.MaxPageSize
        };

        return request;
    }
}