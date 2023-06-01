using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class EmailVerification : PageModel
{
    private readonly ISessionService _sessionService;
    private readonly IHostEnvironment _hostEnvironment;

    public EmailVerification(ISessionService sessionService, IHostEnvironment hostEnvironment)
    {
        _sessionService = sessionService;
        _hostEnvironment = hostEnvironment;
    }
    [BindProperty] public EmailVerificationModel Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(EmailVerificationModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        if (!_hostEnvironment.IsProduction())
        {
            data.PasscodeForTesting = await _sessionService.GetAsync<int?>(SessionKeyConstants.EmailValidationPasscode);
        }

        ModelState.Clear();

        return Page();
    }
    public async Task<IActionResult> OnPostAsync(EmailVerificationModel data)
    {
        var passcode = await _sessionService.GetAsync<int?>(SessionKeyConstants.EmailValidationPasscode);
        var emailToBeValidated = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EmailToBeValidated);

        if (passcode == null || string.IsNullOrWhiteSpace(emailToBeValidated))
            return RedirectToPage("/Session/Timeout");

        Data = data;
        if (ModelState.IsValid)
        {
            if(passcode != data.Passcode)
            {
                ModelState.AddModelError("Data.Passcode", "Enter a correct passcode");
                return Page();
            }

            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquirerEmail, emailToBeValidated);

            await _sessionService.SetAsync<int?>(SessionKeyConstants.EmailValidationPasscode, null);
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EmailToBeValidated, string.Empty);

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(TutoringLogistics), new SearchModel(data));
        }

        return Page();
    }
}