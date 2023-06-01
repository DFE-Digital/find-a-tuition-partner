using Application.Commands.Enquiry.Build;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;
using Domain.Exceptions;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace UI.Pages.Enquiry.Build;

public class EnquirerEmail : PageModel
{
    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;
    private readonly FeatureFlags _featureFlagsConfig;

    public EnquirerEmail(IMediator mediator,
        ISessionService sessionService,
        IOptions<FeatureFlags> featureFlagsConfig)
    {
        _mediator = mediator;
        _sessionService = sessionService;
        _featureFlagsConfig = featureFlagsConfig.Value;
    }
    [BindProperty] public EnquirerEmailModel Data { get; set; } = new();

    [ViewData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(EnquirerEmailModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        ErrorMessage = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquirerEmailErrorMessage);

        Data.Email = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EmailToBeVerified);

        if (string.IsNullOrEmpty(Data.Email))
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
            var errorMessage = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquirerEmailErrorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquirerEmailErrorMessage, string.Empty);
            }

            var previouslyVerifiedEmail = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquirerEmail);

            if (!_featureFlagsConfig.VerifyEmail ||
                previouslyVerifiedEmail != null && previouslyVerifiedEmail.Equals(data.Email!, StringComparison.OrdinalIgnoreCase))
            {
                if (!_featureFlagsConfig.VerifyEmail)
                {
                    await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquirerEmail, data.Email!);
                }

                if (data.From == ReferrerList.CheckYourAnswers)
                {
                    return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
                }

                return RedirectToPage(nameof(TutoringLogistics), new SearchModel(data));
            }
            else
            {
                var sendEmailVerificationCommand = new SendEmailVerificationCommand()
                {
                    Email = data.Email!,
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
                    ModelState.AddModelError("ErrorMessage", "There was a problem sending the email and you should check the email address and try again");
                    return Page();
                }

                await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EmailVerificationPasscode, passcode.Value.ToString());
                await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EmailToBeVerified, data.Email!);

                return RedirectToPage(nameof(EmailVerification), new SearchModel(data));
            }
        }

        return Page();
    }
}