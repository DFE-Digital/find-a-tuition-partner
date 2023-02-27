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
        //TODO - Check if this is needed, if it is then it's repeated from search and compare - change this so not repeated code, add to a base class or extesntion method
        if (data.KeyStages == null && data.Subjects != null)
        {
            data.KeyStages = Enum.GetValues(typeof(KeyStage)).Cast<KeyStage>()
                .Where(x => string.Join(" ", data.Subjects).Contains(x.ToString())).ToArray();
        }
        Data = data;
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        //TODO - Move to Application project in same way as others rather than calling cross page
        //TODO - Only need the results, not the extra data and ordering etc - pass in flag?
        //TODO - Will we also pass in the group sizes to this filter, even though not used on search page?
        var searchResultsData = new SearchResults.Query(Data);
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

        //TODO - deal with errors if fails and if zero TPs to send enquiry to
        return Page();
    }
}