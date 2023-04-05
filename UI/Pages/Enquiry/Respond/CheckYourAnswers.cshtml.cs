using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond;

public class CheckYourAnswers : ResponsePageModel<CheckYourAnswers>
{
    [BindProperty] public CheckYourAnswersModel Data { get; set; } = new();

    [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

    [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;


    private readonly IHostEnvironment _hostEnvironment;

    public CheckYourAnswers(ISessionService sessionService, IMediator mediator, IHostEnvironment hostEnvironment) : base(sessionService, mediator)
    {
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> OnGetAsync(CheckYourAnswersModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync(GetSessionKey(TuitionPartnerSeoUrl!, SupportReferenceNumber)))
            return RedirectToPage("/Session/Timeout");

        var queryToken = Request.Query["Token"].ToString();

        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, TuitionPartnerSeoUrl, true));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        Data = data;

        Data.SupportReferenceNumber = SupportReferenceNumber;
        Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
        Data.Token = queryToken;

        var sessionValues = await _sessionService.RetrieveDataAsync(GetSessionKey(data.TuitionPartnerSeoUrl!, data.SupportReferenceNumber));

        foreach (var sessionValue in sessionValues!)
        {
            Data.EnquiryResponseParseSessionValues(sessionValue.Key, sessionValue.Value);
        }
        HttpContext.AddLadNameToAnalytics<CheckYourAnswers>(Data.LocalAuthorityDistrict);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _sessionService.SessionDataExistsAsync(GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber)))
            return RedirectToPage("/Session/Timeout");

        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, Data.SupportReferenceNumber, Data.TuitionPartnerSeoUrl, true));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        if (!ModelState.IsValid) return Page();

        Data.BaseServiceUrl = Request.GetBaseServiceUrl();

        var command = new AddEnquiryResponseCommand()
        {
            Data = Data
        };

        var responseConfirmationModel = await _mediator.Send(command);

        await _sessionService.DeleteDataAsync(GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber));

        var enquirerMagicLinkQueryString = string.Empty;
        if (!_hostEnvironment.IsProduction())
        {
            enquirerMagicLinkQueryString = $"&EnquirerMagicLink={responseConfirmationModel.EnquirerMagicLink}";
        }

        HttpContext.AddLadNameToAnalytics<CheckYourAnswers>(Data.LocalAuthorityDistrict);
        HttpContext.AddTuitionPartnerNameToAnalytics<CheckYourAnswers>(responseConfirmationModel.TuitionPartnerName);
        HttpContext.AddEnquirySupportReferenceNumberToAnalytics<CheckYourAnswers>(Data.SupportReferenceNumber);

        var redirectPageUrl = $"/enquiry-response/{Data.TuitionPartnerSeoUrl}/{Data.SupportReferenceNumber}/confirmation?Token={Data.Token}&LocalAuthorityDistrictName={Data.LocalAuthorityDistrict}{enquirerMagicLinkQueryString}";
        return Redirect(redirectPageUrl);
    }
}