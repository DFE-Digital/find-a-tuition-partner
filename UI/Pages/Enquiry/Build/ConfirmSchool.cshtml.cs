using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class ConfirmSchool : PageModel
{
    private readonly ISessionService _sessionService;
    private readonly IMediator _mediator;

    public ConfirmSchool(ISessionService sessionService, IMediator mediator)
    {
        _sessionService = sessionService;
        _mediator = mediator;
    }
    [BindProperty] public ConfirmSchoolModel Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(ConfirmSchoolModel data)
    {
        Data = data;

        Data.Email = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquirerEmail);

        return Page();
    }
    public async Task<IActionResult> OnPostAsync(ConfirmSchoolModel data)
    {
        Data = data;
        if (ModelState.IsValid)
        {
            //TODO - Ensure that run same postcode valdation (e.g. is English postcode) as index page

            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquirerEmail, data.Email!);

            var errorMessage = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquirerEmailErrorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquirerEmailErrorMessage, string.Empty);
            }

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(TutoringLogistics), new SearchModel(data));
        }

        return Page();
    }
}