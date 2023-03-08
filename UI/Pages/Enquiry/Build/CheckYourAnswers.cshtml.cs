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

    public async Task<IActionResult> OnGet(CheckYourAnswersModel data)
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

        if (Data.KeyStages == null) return RedirectToPage("../../WhichKeyStages");

        //TODO - This is currently using the subjects string, which contains key stage and subject data, but is not the display version
        Data.KeyStageSubjects = GetKeyStageSubject(string.Join(",", Data.Subjects!));

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data.KeyStageSubjects = GetKeyStageSubject(string.Join(",", Data.Subjects!));

        if (!ModelState.IsValid) return Page();

        var searchResultsData = new GetSearchResultsQuery(Data);
        var searchResults = await _mediator.Send(searchResultsData);
        Data = Data with { TuitionPartnersForEnquiry = searchResults.Results };

        Data.BaseServiceUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

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

            case var k when k.Contains(StringConstants.EnquiryText):
                Data.EnquiryText = value;
                break;
        }
    }
    private static Dictionary<string, List<string>>? GetKeyStageSubject(string value)
    {
        return value.Split(',')
            .Select(x => x.Split('-'))
            .GroupBy(x => x[0])
            .ToDictionary(x => x.Key, x => x.Select(y => y[1]).ToList());
    }
}