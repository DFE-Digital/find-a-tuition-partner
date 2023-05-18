using Application.Common.Models;
using Application.Validators;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using TuitionSetting = Domain.Enums.TuitionSetting;

namespace UI.Pages;
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class SearchResults : PageModel
{
    private readonly IMediator _mediator;
    private readonly FeatureFlags _featureFlagsConfig;
    public SearchResults(IMediator mediator, IOptions<FeatureFlags> featureFlagsConfig)
    {
        _mediator = mediator;
        _featureFlagsConfig = featureFlagsConfig.Value;
    }

    public SearchResultsModel Data { get; set; } = new();
    [BindProperty(SupportsGet = true)] public List<string> CompareListedTuitionPartners { get; set; } = new();
    public List<SelectableTuitionPartnerModel> SelectableTuitionPartners { get; set; } = new();
    public int TotalCompareListedTuitionPartners { get; set; }
    [BindProperty(SupportsGet = true)] public string? UpdateMyCompareList { get; set; }

    public bool IncludeEnquiryBuilder { get; set; } = true;

    // Name keys : Tps => TuitionPartners,Tp => TuitionPartner

    public async Task<IActionResult> OnGet(GetSearchResultsQuery data)
    {
        return await CommonOnGetPostLogic(data);
    }

    public async Task<IActionResult> OnPost(GetSearchResultsQuery data)
    {
        return await CommonOnGetPostLogic(data);
    }

    public async Task<IActionResult> OnGetClearAllFilters(string postcode)
    {
        return await CommonOnGetPostLogic(new GetSearchResultsQuery { Postcode = postcode });
    }

    private async Task<IActionResult> CommonOnGetPostLogic(GetSearchResultsQuery data)
    {
        IncludeEnquiryBuilder = _featureFlagsConfig.EnquiryBuilder;

        data.TuitionSetting ??= TuitionSetting.NoPreference;
        data.KeyStages = data.KeyStages.UpdateFromSubjects(data.Subjects);

        Data = new SearchResultsModel(data)
        {
            AllSubjects = await _mediator.Send(new GetWhichSubjectQuery
            {
                Subjects = data.Subjects,
            }),
            AllTuitionSettings = EnumExtensions.GetAllEnums<TuitionSetting>()
        };

        ModelState.Clear();
        MapErrors(await new SearchResultValidator().ValidateAsync(data));
        if (!ModelState.IsValid) return Page();

        var searchResultData = await _mediator.Send(data);
        MapErrors(searchResultData.Validation);
        if (!ModelState.IsValid) return Page();

        Data.Results = searchResultData.Results;

        Data.From = ReferrerList.SearchResults;

        //Clear compare list TuitionSetting if has been changed on compare list
        if (data.PreviousTuitionSetting != null && data.PreviousTuitionSetting != data.TuitionSetting)
        {
            Data.CompareListTuitionSetting = null;
        }
        Data.PreviousTuitionSetting = data.TuitionSetting;

        await SetSelectableTuitionPartners();

        HttpContext.AddLadNameToAnalytics<SearchResults>(Data.Results);

        return Page();
    }

    private void MapErrors(ValidationResult? validationResult)
    {
        if (validationResult != null && !validationResult.IsValid)
            foreach (var error in validationResult.Errors)
                ModelState.AddModelError($"Data.{error.PropertyName}", error.ErrorMessage);
    }

    private async Task SetSelectableTuitionPartners()
    {
        if (Data.Results != null)
            SelectableTuitionPartners = await GetSelectableTuitionPartners(Data, CompareListedTuitionPartners);
    }

    private async Task<List<SelectableTuitionPartnerModel>> GetSelectableTuitionPartners(
        SearchResultsModel resultsModel, IEnumerable<string> selectedTuitionPartners)
    {
        var selectableTuitionPartnerModels = new List<SelectableTuitionPartnerModel>();
        selectedTuitionPartners = selectedTuitionPartners.ToList();

        var selectableTps = resultsModel.Results?.Results.Select(tp => tp.SeoUrl).ToList();
        var compareListedTps = (await _mediator.Send(new GetAllCompareListTuitionPartnersQuery())).ToList();
        var compareListedTpsInSelectableTps = new List<string>();
        if (compareListedTps.Any() && selectableTps != null && selectableTps.Any())
        {
            compareListedTpsInSelectableTps =
                GetSelectableTpsPresentInCompareListedTps(selectableTps, compareListedTps).ToList();

            selectableTuitionPartnerModels.AddRange(compareListedTpsInSelectableTps
                .Select(tpSeoUrl => new SelectableTuitionPartnerModel(tpSeoUrl, true)));
        }


        if (!string.IsNullOrEmpty(UpdateMyCompareList))
        {
            await UpdateTuitionPartners(selectedTuitionPartners, compareListedTpsInSelectableTps, compareListedTps);

            selectableTuitionPartnerModels.Clear();
            selectableTuitionPartnerModels.AddRange(selectedTuitionPartners
                .Select(tpSeoUrl => new SelectableTuitionPartnerModel(tpSeoUrl, true)));
        }

        TotalCompareListedTuitionPartners = compareListedTps.Count;

        return GetSelectableTuitionPartnerModels(selectableTps, selectableTuitionPartnerModels);
    }

    private IEnumerable<string> GetSelectableTpsPresentInCompareListedTps(List<string> selectableTps,
        List<string> compareListedTps) =>
        selectableTps.Where(tp => compareListedTps.Any(stp => stp == tp)).ToList();


    private async Task UpdateTuitionPartners(
        IEnumerable<string> selectedTuitionPartners, List<string> compareListedTpsInSelectableTps,
        List<string> compareListedTps)
    {
        if (compareListedTpsInSelectableTps.Any())
        {
            foreach (var tp in compareListedTpsInSelectableTps.Where(tp => compareListedTps.Any(stp => stp == tp)))
            {
                compareListedTps.RemoveAll(stp => stp == tp);
            }
        }

        compareListedTps.AddRange(selectedTuitionPartners);

        if (compareListedTps.Any())
            await _mediator.Send(new AddTuitionPartnersToCompareListCommand(compareListedTps));

        if (!compareListedTps.Any())
            await _mediator.Send(new RemoveAllCompareListedTuitionPartnersCommand());
    }

    private List<SelectableTuitionPartnerModel> GetSelectableTuitionPartnerModels(
        List<string>? selectableTps, List<SelectableTuitionPartnerModel> selectableTuitionPartnerModels)
    {
        var tuitionPartners = selectableTps?.Select(seoUrl =>
            new SelectableTuitionPartnerModel(seoUrl)).ToList();

        if (tuitionPartners == null) return selectableTuitionPartnerModels;
        //Set up returned value to have the selected option set correctly
        tuitionPartners = tuitionPartners
            .Except(selectableTuitionPartnerModels, new SelectableTuitionPartnerModelComparer()).ToList();
        //Add to list returned which is what the user eventually sees.
        selectableTuitionPartnerModels.AddRange(tuitionPartners);

        return selectableTuitionPartnerModels;
    }
}