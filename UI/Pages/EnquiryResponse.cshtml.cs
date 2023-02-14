using Application.Common.Interfaces;
using Application.Common.Models;

namespace UI.Pages;

public class EnquiryResponse : PageModel
{
    private readonly IMediator _mediator;
    private readonly IEncrypt _aesEncrypt;

    public EnquiryResponse(IMediator mediator, IEncrypt aesEncrypt)
    {
        _mediator = mediator;
        _aesEncrypt = aesEncrypt;
    }

    [BindProperty] public EnquiryResponseModel Data { get; set; } = new();

    [TempData] public string SuccessMessage { get; set; } = null!;

    [TempData] public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        var errorMessage = "Invalid Url";

        var token = Request.Query["token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            ErrorMessage = errorMessage;

            ModelState.AddModelError("Data.ErrorMessage", errorMessage);

            return Page();
        }

        try
        {
            GetEnquiryIdFromToken(token);
        }
        catch
        {
            errorMessage = "Invalid token provided in the URl.";

            ErrorMessage = errorMessage;

            ModelState.AddModelError("Data.ErrorMessage", errorMessage);

            return Page();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        var token = Request.Query["token"];

        try
        {
            var enquiryId = GetEnquiryIdFromToken(token);

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
            var errorMessage = "Invalid token provided in the URl.";
            ErrorMessage = errorMessage;
            ModelState.AddModelError("Data.ErrorMessage", errorMessage);
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
}