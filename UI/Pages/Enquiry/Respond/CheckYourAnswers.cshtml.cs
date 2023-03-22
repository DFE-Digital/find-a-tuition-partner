using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond;

public class CheckYourAnswers : PageModel
{
    [BindProperty] public CheckYourAnswersModel Data { get; set; } = new();

    private readonly ISessionService _sessionService;
    private readonly IMediator _mediator;
    private readonly IEncrypt _aesEncrypt;
    private readonly IHostEnvironment _hostEnvironment;

    private const string InvalidTokenErrorMessage = "Invalid token provided in the URl.";

    private const string InvalidUrlErrorMessage = "Invalid Url";

    [ViewData] public string? ErrorMessage { get; set; }

    public CheckYourAnswers(ISessionService sessionService, IMediator mediator, IEncrypt aesEncrypt, IHostEnvironment hostEnvironment)
    {
        _sessionService = sessionService;
        _mediator = mediator;
        _aesEncrypt = aesEncrypt;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> OnGetAsync(CheckYourAnswersModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        var sessionValues = await _sessionService.RetrieveDataAsync();

        if (sessionValues != null)
        {
            foreach (var sessionValue in sessionValues)
            {
                Data.EnquiryResponseParseSessionValues(sessionValue.Key, sessionValue.Value);
            }
            HttpContext.AddLadNameToAnalytics(Data.LocalAuthorityDistrict);
            HttpContext.AddTuitionPartnerNameToAnalytics(Data.TuitionPartnerName);
            HttpContext.AddEnquirySupportReferenceNumberToAnalytics(Data.SupportReferenceNumber);
        }

        var getMagicLinkToken = await GetMagicLinkToken(Data.Token);
        if (getMagicLinkToken == null) return Page();

        ParsedTokenValuesFromToken(Data.Token);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        if (!ModelState.IsValid) return Page();

        var getMagicLinkToken = await GetMagicLinkToken(Data.Token);
        if (getMagicLinkToken == null) return Page();

        ParsedTokenValuesFromToken(Data.Token);

        Data.BaseServiceUrl = Request.GetBaseServiceUrl();

        var command = new AddEnquiryResponseCommand()
        {
            Data = Data
        };

        var submittedConfirmationModel = await _mediator.Send(command);

        if (!string.IsNullOrEmpty(submittedConfirmationModel.SupportReferenceNumber))
        {
            await _sessionService.DeleteDataAsync();

            submittedConfirmationModel.LocalAuthorityDistrictName = Data.LocalAuthorityDistrict;
            submittedConfirmationModel.TuitionPartnerName = Data.TuitionPartnerName;

            if (_hostEnvironment.IsProduction())
            {
                submittedConfirmationModel.EnquirerMagicLink = string.Empty;
            }

            HttpContext.AddLadNameToAnalytics(Data.LocalAuthorityDistrict);
            HttpContext.AddTuitionPartnerNameToAnalytics(Data.TuitionPartnerName);
            HttpContext.AddEnquirySupportReferenceNumberToAnalytics(Data.SupportReferenceNumber);

            return RedirectToPage(nameof(ResponseConfirmation), submittedConfirmationModel);
        }

        return Page();
    }

    private void ParsedTokenValuesFromToken(string token)
    {
        string tokenValue;

        try
        {
            tokenValue = _aesEncrypt.Decrypt(token);
        }
        catch
        {
            var parsedToken = ParseTokenFromQueryString();

            try
            {
                tokenValue = _aesEncrypt.Decrypt(parsedToken);
            }
            catch
            {
                tokenValue = string.Empty;
            }
        }

        if (string.IsNullOrEmpty(tokenValue))
        {
            AddErrorMessage(InvalidUrlErrorMessage);
            return;
        }

        var splitTokenValue = tokenValue.Split('&', StringSplitOptions.RemoveEmptyEntries);

        if (!splitTokenValue.Any()) return;

        var splitTokenTypePart = splitTokenValue[0].Split('=', StringSplitOptions.RemoveEmptyEntries);

        var tokenType = splitTokenTypePart[1];

        if (!string.IsNullOrWhiteSpace(tokenType) && tokenType != nameof(MagicLinkType.EnquiryRequest))
        {
            AddErrorMessage(InvalidUrlErrorMessage);
            return;
        }

        var splitTuitionPartnerPart = splitTokenValue[1].Split('=', StringSplitOptions.RemoveEmptyEntries);

        if (int.TryParse(splitTuitionPartnerPart[1], out var tuitionPartnerId))
        {
            Data.TuitionPartnerId = tuitionPartnerId;
        }
    }

    private async Task<MagicLinkDto?> GetMagicLinkToken(string token)
    {
        var getMagicLinkTokenQuery = await _mediator.Send(new GetMagicLinkTokenQuery(token, nameof(MagicLinkType.EnquiryRequest)));

        if (getMagicLinkTokenQuery == null)
        {
            AddErrorMessage(InvalidTokenErrorMessage);

            return null;
        }

        Data.EnquiryId = getMagicLinkTokenQuery.EnquiryId!.Value;

        return getMagicLinkTokenQuery;
    }

    private void AddErrorMessage(string errorMessage)
    {
        ErrorMessage = errorMessage;

        ModelState.AddModelError("Data.ErrorMessage", ErrorMessage);
    }

    private string ParseTokenFromQueryString()
    {
        var queryString = Request.QueryString.Value;
        var tokens = queryString!.Split(new char[] { '=' }, 2);
        var tokenValue = tokens[1];
        return tokenValue;
    }
}