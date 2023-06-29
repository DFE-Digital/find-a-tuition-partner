using Application.Commands.Enquiry.Build;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;
using Domain.Exceptions;

namespace UI.Pages.Enquiry.Build;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class EmailVerification : PageModel
{
    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;
    private readonly IHostEnvironment _hostEnvironment;

    public EmailVerification(IMediator mediator,
        ISessionService sessionService,
        IHostEnvironment hostEnvironment)
    {
        _mediator = mediator;
        _sessionService = sessionService;
        _hostEnvironment = hostEnvironment;
    }
    [BindProperty] public EmailVerificationModel Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(EmailVerificationModel data)
    {
        var emailToBeVerified = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EmailToBeVerified);

        if (string.IsNullOrWhiteSpace(emailToBeVerified))
            return RedirectToPage("/Session/Timeout");

        Data = data;

        Data.Email = emailToBeVerified;

        if (!_hostEnvironment.IsProduction())
        {
            data.PasscodeForTesting = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EmailVerificationPasscode);
        }

        ModelState.Clear();

        return Page();
    }
    public async Task<IActionResult> OnPostAsync(EmailVerificationModel data)
    {
        var passcode = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EmailVerificationPasscode);
        var emailToBeVerified = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EmailToBeVerified);

        if (string.IsNullOrWhiteSpace(passcode) || string.IsNullOrWhiteSpace(emailToBeVerified))
            return RedirectToPage("/Session/Timeout");

        Data = data;
        if (ModelState.IsValid)
        {
            if (!passcode.Equals(data.Passcode))
            {
                ModelState.AddModelError("Data.Passcode", "Enter a correct passcode");
                return Page();
            }

            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquirerEmail, emailToBeVerified);

            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EmailVerificationPasscode, string.Empty);
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EmailToBeVerified, string.Empty);

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(TutoringLogistics), new SearchModel(data));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostNewPasscodeAsync(EmailVerificationModel data)
    {
        var emailToBeVerified = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EmailToBeVerified);

        if (string.IsNullOrWhiteSpace(emailToBeVerified))
            return RedirectToPage("/Session/Timeout");

        var sendEmailVerificationCommand = new SendEmailVerificationCommand()
        {
            Email = emailToBeVerified,
            PasscodeLength = Constants.IntegerConstants.EmailVerificationPasscodeLength,
            SessionTimeoutMinutes = Constants.DoubleConstants.SessionTimeoutInMinutes,
            BaseServiceUrl = Request.GetBaseServiceUrl()
        };

        int? passcode;

        try
        {
            passcode = await _mediator.Send(sendEmailVerificationCommand);
        }
        catch (EmailSendException)
        {
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquirerEmailErrorMessage, Constants.StringConstants.EmailErrorMessage);

            return RedirectToPage(nameof(EnquirerEmail), new SearchModel(Data));
        }

        await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EmailVerificationPasscode, passcode.Value.ToString());

        data.NewPasscodeSentAt = DateTime.UtcNow.ToLocalDateTime().ToString("h:mmtt");
        data.Passcode = null;
        data.PasscodeForTesting = null;
        data.Email = null;

        return RedirectToPage(data);
    }
}