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

    [TempData] public string SuccessMessage { get; set; } = null!;

    [TempData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var token = Request.Query["token"];

        if (AddInValidUrlErrorMessage(token)) return Page();

        try
        {
            GetEnquiryIdFromToken(token);

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

        var token = Request.Query["token"];

        if (AddInValidUrlErrorMessage(token)) return Page();

        try
        {
            var enquiryId = GetEnquiryIdFromToken(token);

            var validMagicLinkToken = await IsValidMagicLinkToken(token);
            if (!validMagicLinkToken) return Page();

            Data.EnquiryId = enquiryId;

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

    private int GetEnquiryIdFromToken(string token)
    {
        var tokenValue = _aesEncrypt.Decrypt(token);

        var splitTokenValue = tokenValue.Split('&', StringSplitOptions.RemoveEmptyEntries);

        var splitEnquiryPart = splitTokenValue[0].Split('=', StringSplitOptions.RemoveEmptyEntries);

        if (int.TryParse(splitEnquiryPart[1], out var enquiryId))
        {
            Data.EnquiryId = enquiryId;
        }

        return enquiryId;
    }

    private void AddErrorMessage(string errorMessage)
    {
        ErrorMessage = errorMessage;

        ModelState.AddModelError("Data.ErrorMessage", errorMessage);
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