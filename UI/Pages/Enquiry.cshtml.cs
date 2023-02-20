using Application.Common.Models;

namespace UI.Pages;

public class Enquiry : PageModel
{
    private readonly IMediator _mediator;

    public Enquiry(IMediator mediator) => _mediator = mediator;
    [BindProperty] public EnquiryModel Data { get; set; } = new();

    [ViewData] public string SuccessMessage { get; set; } = null!;

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        var compareListedTpsQuery = new GetAllCompareListTuitionPartnersQuery();

        var compareListedTps = await _mediator.Send(compareListedTpsQuery);

        if (compareListedTps.Any())
        {
            Data.SelectedTuitionPartners = compareListedTps.ToList();
        }
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

            SuccessMessage = $"Enquiry successfully sent to the compare listed tuition partners. You will receive an email shortly to the following email: {Data.Email} to view the enquiry response.";
            ModelState.Clear();
            Data = new();
        }

        return Page();
    }
}