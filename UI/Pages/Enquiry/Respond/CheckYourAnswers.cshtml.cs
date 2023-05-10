using Application.Commands.Enquiry.Respond;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Application.Queries.Enquiry;
using UI.Models;

namespace UI.Pages.Enquiry.Respond;

public class CheckYourAnswers : ResponsePageModel<CheckYourAnswers>
{
    [BindProperty] public CheckYourAnswersModel Data { get; set; } = new();

    [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

    [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

    private const string EnquiryResponseConfirmationModelKey = "EnquirySubmissionConfirmationModel";
    private const string EnquiryResponseFormPostTimestampKey = "EnquiryResponseFormPostTimestampKey";

    private readonly IHostEnvironment _hostEnvironment;

    public CheckYourAnswers(ISessionService sessionService, IMediator mediator, IHostEnvironment hostEnvironment) : base(sessionService, mediator)
    {
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> OnGetAsync(CheckYourAnswersModel data)
    {
        var queryToken = Request.Query["Token"].ToString();

        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, TuitionPartnerSeoUrl, true));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        if (!await _sessionService.SessionDataExistsAsync(GetSessionKey(TuitionPartnerSeoUrl!, SupportReferenceNumber)))
            return RedirectToPage("/Session/Timeout");

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
        var isValidMagicLink =
            await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, Data.SupportReferenceNumber, Data.TuitionPartnerSeoUrl, true));

        if (!isValidMagicLink)
        {
            return NotFound();
        }

        if (!await _sessionService.SessionDataExistsAsync(GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber)))
            return RedirectToPage("/Session/Timeout");

        if (!ModelState.IsValid) return Page();

        Data.BaseServiceUrl = Request.GetBaseServiceUrl();

        var command = new AddEnquiryResponseCommand()
        {
            Data = Data
        };

        ResponseConfirmationModel? responseConfirmationModel;

        var enquiryResponseConfirmationModelKey = $"{EnquiryResponseConfirmationModelKey}-{Data.SupportReferenceNumber}";
        var enquiryResponseFormPostTimestampKey = $"{EnquiryResponseFormPostTimestampKey}-{Data.SupportReferenceNumber}";

        if (!await _sessionService.IsDuplicateFormPostAsync(enquiryResponseFormPostTimestampKey))
        {
            responseConfirmationModel = await _mediator.Send(command);
            await _sessionService.SetFormPostResponse(responseConfirmationModel, enquiryResponseConfirmationModelKey);
        }
        else
        {
            responseConfirmationModel = await _sessionService.GetPreviousFormPostResponse<ResponseConfirmationModel>(enquiryResponseConfirmationModelKey);
        }

        await _sessionService.DeleteDataAsync(GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber));

        var enquirerMagicLinkQueryString = string.Empty;
        if (!_hostEnvironment.IsProduction())
        {
            enquirerMagicLinkQueryString = $"&EnquirerMagicLink={responseConfirmationModel.EnquirerMagicLink}";
        }

        HttpContext.AddLadNameToAnalytics<CheckYourAnswers>(Data.LocalAuthorityDistrict);

        var redirectPageUrl = $"/enquiry-response/{Data.TuitionPartnerSeoUrl}/{Data.SupportReferenceNumber}/confirmation?Token={Data.Token}&LocalAuthorityDistrictName={Data.LocalAuthorityDistrict}{enquirerMagicLinkQueryString}";
        return Redirect(redirectPageUrl);
    }
}