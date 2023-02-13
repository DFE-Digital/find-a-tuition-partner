using Application.Common.Models;

namespace UI.Pages;

public class Enquiry : PageModel
{
    private readonly IMediator _mediator;

    public Enquiry(IMediator mediator) => _mediator = mediator;
    [BindProperty] public EnquiryModel Data { get; set; } = new();

    [TempData] public string Message { get; set; } = null!;

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

        var dataSaved = await _mediator.Send(command);

        if (dataSaved)
        {
            var sendEnquiryEmailCommand = new SendEnquiryEmailCommand()
            {
                Data = Data
            };

            await _mediator.Send(sendEnquiryEmailCommand);

            Message = $"Enquiry successfully sent to the compare listed tuition partners. You will receive a confirmation email shortly to the following email: {Data.Email}";
            ModelState.Clear();
            Data = new();
        }

        return Page();
    }
}