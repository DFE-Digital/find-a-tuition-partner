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
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        ErrorMessage = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquirerEmailErrorMessage);

        Data.Email = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquirerEmail);

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError("ErrorMessage", ErrorMessage);
            return Page();
        }

        ModelState.Clear();

        return Page();
    }
    public async Task<IActionResult> OnPostAsync(EnquirerEmailModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;
        if (ModelState.IsValid)
        {
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