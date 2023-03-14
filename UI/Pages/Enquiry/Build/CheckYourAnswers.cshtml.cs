using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class CheckYourAnswers : PageModel
{
    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;

    public CheckYourAnswers(IMediator mediator, ISessionService sessionService)
    {
        _mediator = mediator;
        _sessionService = sessionService;
    }

    [BindProperty] public CheckYourAnswersModel Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(CheckYourAnswersModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        var sessionValues = await _sessionService.RetrieveDataAsync();

        if (sessionValues != null)
        {
            foreach (var sessionValue in sessionValues)
            {
                ParseSessionValue(sessionValue.Key, sessionValue.Value);
            }
        }

        //TODO - Test and handle errors:
        //  No postcode, subjects, TT. email, logistics etc
        //  Invalid data supplied - postcode in Wales, invalid email etc
        //  errors when calling _mediator

        Data.KeyStageSubjects = GetKeyStageSubject(string.Join(",", Data.Subjects!));

        if (!string.IsNullOrWhiteSpace(Data.Postcode))
        {
            var locationResult = await _mediator.Send(new GetSearchLocationQuery(Data.Postcode));
            Data.LocalAuthorityDistrictName = locationResult == null ? string.Empty : locationResult.LocalAuthorityDistrict;
        }

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data.KeyStageSubjects = GetKeyStageSubject(string.Join(",", Data.Subjects!));

        if (!ModelState.IsValid) return Page();

        var searchResultsData = new GetSearchResultsQuery(Data);
        var searchResults = await _mediator.Send(searchResultsData);
        Data = Data with { TuitionPartnersForEnquiry = searchResults.Results };

        Data.BaseServiceUrl = Request.GetBaseServiceUrl();

        var command = new AddEnquiryCommand()
        {
            Data = Data
        };

        var supportReferenceNumber = await _mediator.Send(command);

        if (!string.IsNullOrEmpty(supportReferenceNumber))
        {
            Data.SupportReferenceNumber = supportReferenceNumber;

            await _sessionService.DeleteDataAsync();

            return RedirectToPage(nameof(SubmittedConfirmation), new SearchModel(Data));
        }

        return Page();
    }

    private void ParseSessionValue(string key, string value)
    {
        switch (key)
        {
            case var k when k.Contains(StringConstants.EnquirerEmail):
                Data.Email = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryTutoringLogistics):
                Data.TutoringLogistics = value;
                break;

            case var k when k.Contains(StringConstants.EnquirySENDRequirements):
                Data.SENDRequirements = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryAdditionalInformation):
                Data.AdditionalInformation = value;
                break;
        }
    }
    private static Dictionary<KeyStage, List<Subject>> GetKeyStageSubject(string value)
    {
        var keyStageSubjects = new Dictionary<KeyStage, List<Subject>>();

        var allKeyStages = Enum.GetValues(typeof(KeyStage)).Cast<KeyStage>();

        var allSubjects = Enum.GetValues(typeof(Subject)).Cast<Subject>();

        var groupedByKeyStage = value.Split(',')
            .Select(x => x.Split('-'))
            .GroupBy(x => x[0])
            .ToDictionary(x => x.Key, x => string.Join(',', x.Select(y => y[1]).ToList()))
            .OrderBy(x => x.Key);

        foreach (var kvp in groupedByKeyStage)
        {
            var keyStage = allKeyStages.Where(x => x.ToString().ToSeoUrl() == kvp.Key.ToSeoUrl());
            if (keyStage?.Count() == 1)
            {
                var subjects = allSubjects.Where(x => kvp.Value.ToSeoUrl().Contains(x.ToString().ToSeoUrl())).ToList();
                if (subjects?.Count > 0)
                {
                    keyStageSubjects.Add(keyStage.First(), subjects);
                }
            }
        }

        return keyStageSubjects;
    }
}