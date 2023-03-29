using System.Net;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond;

public class CheckYourAnswers : ResponsePageModel<CheckYourAnswers>
{
    [BindProperty] public CheckYourAnswersModel Data { get; set; } = new();

    private readonly IHostEnvironment _hostEnvironment;

    public CheckYourAnswers(ISessionService sessionService, IMediator mediator, IHostEnvironment hostEnvironment) : base(sessionService, mediator)
    {
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> OnGetAsync(CheckYourAnswersModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync(GetSessionKey(data.TuitionPartnerSeoUrl!, data.SupportReferenceNumber)))
            return RedirectToPage("/Session/Timeout");

        Data = data;

        var sessionValues = await _sessionService.RetrieveDataAsync(GetSessionKey(data.TuitionPartnerSeoUrl!, data.SupportReferenceNumber));

        if (sessionValues != null)
        {
            foreach (var sessionValue in sessionValues)
            {
                Data.EnquiryResponseParseSessionValues(sessionValue.Key, sessionValue.Value);
            }
            HttpContext.AddLadNameToAnalytics<CheckYourAnswers>(Data.LocalAuthorityDistrict);
        }

        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, Data.SupportReferenceNumber, true));

        if (!isValidMagicLink)
        {
            TempData["Status"] = HttpStatusCode.NotFound;
            return RedirectToPage(nameof(ErrorModel));
        }

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _sessionService.SessionDataExistsAsync(GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber)))
            return RedirectToPage("/Session/Timeout");

        if (!ModelState.IsValid) return Page();

        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, Data.SupportReferenceNumber, true));

        if (!isValidMagicLink)
        {
            TempData["Status"] = HttpStatusCode.NotFound;
            return RedirectToPage(nameof(ErrorModel));
        }

        Data.BaseServiceUrl = Request.GetBaseServiceUrl();

        var command = new AddEnquiryResponseCommand()
        {
            Data = Data
        };

        var submittedConfirmationModel = await _mediator.Send(command);

        if (!submittedConfirmationModel.IsValid && submittedConfirmationModel.ErrorStatus == HttpStatusCode.NotFound.ToString())
        {
            TempData["Status"] = HttpStatusCode.NotFound;
            return RedirectToPage(nameof(ErrorModel));
        }

        if (!submittedConfirmationModel.IsValid && submittedConfirmationModel.ErrorStatus == HttpStatusCode.InternalServerError.ToString())
        {
            TempData["Status"] = HttpStatusCode.InternalServerError;
            return RedirectToPage(nameof(ErrorModel));
        }

        if (!string.IsNullOrEmpty(submittedConfirmationModel.SupportReferenceNumber))
        {
            await _sessionService.DeleteDataAsync(GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber));

            submittedConfirmationModel.LocalAuthorityDistrictName = Data.LocalAuthorityDistrict;

            if (_hostEnvironment.IsProduction())
            {
                submittedConfirmationModel.EnquirerMagicLink = string.Empty;
            }

            HttpContext.AddLadNameToAnalytics<CheckYourAnswers>(Data.LocalAuthorityDistrict);
            HttpContext.AddTuitionPartnerNameToAnalytics<CheckYourAnswers>(submittedConfirmationModel.TuitionPartnerName);
            HttpContext.AddEnquirySupportReferenceNumberToAnalytics<CheckYourAnswers>(Data.SupportReferenceNumber);

            return RedirectToPage(nameof(ResponseConfirmation), submittedConfirmationModel);
        }

        return Page();
    }
}