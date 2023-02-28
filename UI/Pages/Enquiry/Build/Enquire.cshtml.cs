using Application.Common.Models;

namespace UI.Pages.Enquiry.Build;

public class Enquire : PageModel
{
    private readonly IMediator _mediator;

    public Enquire(IMediator mediator) => _mediator = mediator;
    [BindProperty] public EnquiryModel Data { get; set; } = new();

    public record Query : SearchModel;

    public void OnGet(EnquiryModel data)
    {
        data.TuitionType ??= TuitionType.Any;
        data.KeyStages = data.KeyStages.UpdateFromSubjects(data.Subjects);

        Data = data;
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        //TODO - Will we also need to pass in the group sizes to this filter, even though not used on search page for MVP
        var searchResultsData = new GetSearchResultsQuery(Data);
        var searchResults = await _mediator.Send(searchResultsData);
        Data = Data with { TuitionPartnersForEnquiry = searchResults.Results };

        var command = new AddEnquiryCommand()
        {
            Data = Data
        };

        //TODO - Ideally the whole enquiry creation, magic link and emails would be a unit of work - is it worth changing?  Possibly look to use events on AddEnquiryCommand to send emails rather than SendEnquiryEmailCommand & SendEnquirerViewAllResponsesEmailCommand mediator calls and save unit at end if no errors?
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

        //TODO - deal with errors if fails and if zero TPs returned to send enquiry to
        return Page();
    }
}