using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class EnquirerEmail : PageModel
{
    private readonly ISessionService _sessionService;

    public EnquirerEmail(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    [BindProperty] public EnquirerEmailModel Data { get; set; } = new();

    [ViewData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(EnquirerEmailModel data)
    {
        Data = data;

        ErrorMessage = await _sessionService.RetrieveDataAsync(StringConstants.EnquirerEmailErrorMessage);

        Data.Email = await _sessionService.RetrieveDataAsync(StringConstants.EnquirerEmail);

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            await _sessionService.AddOrUpdateDataAsync(StringConstants.EnquirerInvalidEmailAddress, data.Email!);
            ModelState.AddModelError("ErrorMessage", ErrorMessage);
            return Page();
        }

        ModelState.Clear();

        return Page();
    }
    public async Task<IActionResult> OnPostAsync(EnquirerEmailModel data)
    {
        Data = data;
        if (ModelState.IsValid)
        {
            var errorMessage = await _sessionService.RetrieveDataAsync(StringConstants.EnquirerEmailErrorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                var invalidEmail = await _sessionService.RetrieveDataAsync(StringConstants.EnquirerInvalidEmailAddress);

                if (!string.IsNullOrEmpty(invalidEmail) && invalidEmail.Equals(data.Email, StringComparison.OrdinalIgnoreCase))
                {
                    ErrorMessage = await _sessionService.RetrieveDataAsync(StringConstants.EnquirerEmailErrorMessage);
                    ModelState.AddModelError("ErrorMessage", ErrorMessage);
                    return Page();
                }

                await _sessionService.AddOrUpdateDataAsync(StringConstants.EnquirerEmailErrorMessage, string.Empty);
            }

            await _sessionService.AddOrUpdateDataAsync(StringConstants.EnquirerEmail, data.Email!);

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(TutoringLogistics), new SearchModel(data));
        }

        return Page();
    }
}