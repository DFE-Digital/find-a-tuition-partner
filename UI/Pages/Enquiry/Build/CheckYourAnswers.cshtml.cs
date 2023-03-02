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
        Data.Email = await _sessionService.RetrieveDataAsync(StringConstants.EnquirerEmail);
        Data.EnquiryText = await _sessionService.RetrieveDataAsync(StringConstants.EnquiryText);
        Data.Postcode = await _sessionService.RetrieveDataAsync(StringConstants.PostCode);
        var keyStages = await _sessionService.RetrieveDataAsync(StringConstants.KeyStages);
        if (!string.IsNullOrEmpty(keyStages))
        {
            Data.KeyStages = keyStages.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => Enum.TryParse(x, out KeyStage _)).Select(x => Enum.Parse<KeyStage>(x)).ToArray();
        }
        var subjects = await _sessionService.RetrieveDataAsync(StringConstants.Subjects);
        if (!string.IsNullOrEmpty(subjects))
        {
            Data.Subjects = subjects.Split(",", StringSplitOptions.RemoveEmptyEntries);
            Data.KeyStageSubjects = GetKeyStageSubject(subjects);
        }

        if (Data.KeyStages == null) return RedirectToPage("../../WhichKeyStages");

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        if (!ModelState.IsValid) return Page();

        //TODO - No Tuition Type filter at the moment
        var searchResultsData = new GetSearchResultsQuery(Data);
        var searchResults = await _mediator.Send(searchResultsData);
        Data = Data with { TuitionPartnersForEnquiry = searchResults.Results };

        var command = new AddEnquiryCommand()
        {
            Data = Data
        };

        //TODO - Ideally the whole enquiry creation, magic link and emails would be a unit of work - is it worth changing? Possibly look to use events on AddEnquiryCommand to send emails rather than SendEnquiryEmailCommand & SendEnquirerViewAllResponsesEmailCommand mediator calls and save unit at end if no errors?
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

            await _sessionService.ClearAsync();

            return RedirectToPage(nameof(SubmittedConfirmation), new SearchModel(Data));
        }

        return Page();
    }

    private static Dictionary<string, List<string>>? GetKeyStageSubject(string value)
    {
        return value.Split(',')
            .Select(x => x.Split('-'))
            .GroupBy(x => x[0])
            .ToDictionary(x => x.Key, x => x.Select(y => y[1]).ToList());
    }
}