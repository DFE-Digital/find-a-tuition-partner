using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond
{
    public class Response : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IEncrypt _aesEncrypt;

        private const string InvalidTokenErrorMessage = "Invalid token provided in the URl.";

        private const string InvalidUrlErrorMessage = "Invalid Url";

        public Response(IMediator mediator, IEncrypt aesEncrypt)
        {
            _mediator = mediator;
            _aesEncrypt = aesEncrypt;
        }

        [BindProperty] public EnquiryResponseModel Data { get; set; } = new();

        [ViewData] public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var token = Request.Query["token"].ToString();

            if (AddInValidUrlErrorMessage(token)) return Page();

            try
            {
                if (!IsParsedTokenValues(token)) return Page();

                var getMagicLinkToken = await GetMagicLinkToken(token);
                if (getMagicLinkToken == null) return Page();
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
                if (!IsParsedTokenValues(token)) return Page();

                var getMagicLinkToken = await GetMagicLinkToken(token);
                if (getMagicLinkToken == null) return Page();

                Data.BaseServiceUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

                var command = new AddEnquiryResponseCommand()
                {
                    Data = Data
                };

                var hasDataSaved = await _mediator.Send(command);

                if (hasDataSaved)
                {
                    return RedirectToPage(nameof(ResponseConfirmation));
                }
            }
            catch
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return Page();
            }

            return Page();
        }

        private bool IsParsedTokenValues(string token)
        {
            var tokenValue = _aesEncrypt.Decrypt(token);

            var splitTokenValue = tokenValue.Split('&', StringSplitOptions.RemoveEmptyEntries);

            if (!splitTokenValue.Any()) return false;

            var splitTokenTypePart = splitTokenValue[0].Split('=', StringSplitOptions.RemoveEmptyEntries);

            var tokenType = splitTokenTypePart[1];

            if (!string.IsNullOrWhiteSpace(tokenType) && tokenType != nameof(MagicLinkType.EnquiryRequest))
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return false;
            }

            var splitTuitionPartnerPart = splitTokenValue[1].Split('=', StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(splitTuitionPartnerPart[1], out var tuitionPartnerId))
            {
                Data.TuitionPartnerId = tuitionPartnerId;
            }

            Data.Token = token;

            return true;
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
    }
}