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
        Data = data;

        var sessionId = Request.Cookies[StringConstants.SessionCookieName];

        if (sessionId == null) return RedirectToPage(nameof(EnquirerEmail));

        var sessionValues = await _sessionService.RetrieveDataAsync(sessionId);

        if (sessionValues == null) return RedirectToPage(nameof(EnquirerEmail));

        foreach (var sessionValue in sessionValues)
        {
            ParseSessionValue(sessionValue.Key, sessionValue.Value);
        }

        if (Data.KeyStages == null) return RedirectToPage("../../WhichKeyStages");

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        var searchResultsData = new GetSearchResultsQuery(Data);
        var searchResults = await _mediator.Send(searchResultsData);
        Data = Data with { TuitionPartnersForEnquiry = searchResults.Results };

        var command = new AddEnquiryCommand()
        {
            Data = Data
        };

        var enquiryId = await _mediator.Send(command);

        if (enquiryId != default)
        {
            Data.EnquiryId = enquiryId;

            Data.BaseServiceUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            var sendEnquiryEmailCommand = new SendEnquiryEmailCommand()
            {
                Data = Data
            };

            await _mediator.Send(sendEnquiryEmailCommand);

            var sendEnquirerViewResponsesEmailCommand = new SendEnquirerViewAllResponsesEmailCommand()
            {
                Data = Data
            };

            await _mediator.Send(sendEnquirerViewResponsesEmailCommand);

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

            case var k when k.Contains(StringConstants.PostCode):
                Data.Postcode = value;
                break;

            case var k when k.Contains(StringConstants.KeyStages):
                var keyStages = value.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => Enum.TryParse(x, out KeyStage _)).Select(x => Enum.Parse<KeyStage>(x)).ToArray();

                Data.KeyStages = keyStages;
                break;

            case var k when k.Contains(StringConstants.Subjects):
                var subjects = value.Split(",", StringSplitOptions.RemoveEmptyEntries);
                Data.Subjects = subjects;
                Data.KeyStageSubjects = GetKeyStageSubject(value);
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