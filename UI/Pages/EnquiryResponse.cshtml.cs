using Application.Common.Interfaces;
using Application.Common.Models;

namespace UI.Pages;

public class EnquiryResponse : PageModel
{
    private readonly IMediator _mediator;
    private readonly IEncrypt _aesEncrypt;

    private const string InvalidTokenErrorMessage = "Invalid token provided in the URl.";

    private const string InvalidUrlErrorMessage = "Invalid Url";

    public EnquiryResponse(IMediator mediator, IEncrypt aesEncrypt)
    {
        _mediator = mediator;
        _aesEncrypt = aesEncrypt;
    }

    [BindProperty] public EnquiryResponseModel Data { get; set; } = new();

    [ViewData] public string SuccessMessage { get; set; } = null!;

    [ViewData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var token = Request.Query["token"].ToString();

        if (AddInValidUrlErrorMessage(token)) return Page();

        try
        {
            GetTokenValues(token);

            var validMagicLinkToken = await IsValidMagicLinkToken(token);
            if (!validMagicLinkToken) return Page();
        }
        catch
        {
            AddErrorMessage(InvalidTokenErrorMessage);
            return Page();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        var token = Request.Query["token"].ToString();

        if (AddInValidUrlErrorMessage(token)) return Page();

        try
        {
            GetTokenValues(token);

            var validMagicLinkToken = await IsValidMagicLinkToken(token);
            if (!validMagicLinkToken) return Page();

            var command = new AddEnquiryResponseCommand()
            {
                Data = Data
            };

            var hasDataSaved = await _mediator.Send(command);

            if (hasDataSaved)
            {
                SuccessMessage = $"The enquiry response was submitted successfully.";
                ModelState.Clear();
                Data = new();
            }
        }
        catch
        {
            AddErrorMessage(InvalidTokenErrorMessage);
            return Page();
        }

        return Page();
    }

    private void GetTokenValues(string token)
    {
        var tokenValue = _aesEncrypt.Decrypt(token);

        var splitTokenValue = tokenValue.Split('&', StringSplitOptions.RemoveEmptyEntries);

        var splitEnquiryPart = splitTokenValue[0].Split('=', StringSplitOptions.RemoveEmptyEntries);

        if (int.TryParse(splitEnquiryPart[1], out var enquiryId))
        {
            Data.EnquiryId = enquiryId;
        }

        var splitTuitionPartnerPart = splitTokenValue[1].Split('=', StringSplitOptions.RemoveEmptyEntries);

        if (int.TryParse(splitTuitionPartnerPart[1], out var tuitionPartnerId))
        {
            Data.TuitionPartnerId = tuitionPartnerId;
        }

        Data.Token = token;
    }

    private void AddErrorMessage(string errorMessage)
    {
        ErrorMessage = errorMessage;

        ModelState.AddModelError("Data.ErrorMessage", ErrorMessage);
    }

    private bool AddInValidUrlErrorMessage(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            AddErrorMessage(InvalidUrlErrorMessage);
            return true;
        }

        return false;
    }

    private async Task<bool> IsValidMagicLinkToken(string token)
    {
        var isValidMagicLinkToken = await _mediator.Send(new IsValidMagicLinkTokenQuery(token));

        if (!isValidMagicLinkToken)
        {
            AddErrorMessage(InvalidTokenErrorMessage);

            return false;
        }

        return true;
    }
}