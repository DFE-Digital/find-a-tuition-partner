using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;


namespace UI.Pages.Enquiry.Respond
{
    public class AllEnquirerResponses : PageModel
    {
        private const string InvalidTokenErrorMessage = "Invalid token provided in the URl.";

        private const string InvalidUrlErrorMessage = "Invalid Url";

        private readonly IMediator _mediator;
        private readonly IEncrypt _aesEncrypt;

        public AllEnquirerResponses(IMediator mediator, IEncrypt aesEncrypt)
        {
            _mediator = mediator;
            _aesEncrypt = aesEncrypt;
        }

        [BindProperty] public EnquirerViewAllResponsesModel Data { get; set; } = new();

        [ViewData] public string ErrorMessage { get; set; } = string.Empty;

        string _queryToken = string.Empty;

        public async Task<IActionResult> OnGet()
        {
            _queryToken = Request.Query["token"].ToString();

            if (AddInValidUrlErrorMessage(_queryToken)) return Page();

            try
            {
                var enquiryId = GetEnquiryIdFromToken(_queryToken);

                var validMagicLinkToken = await IsValidMagicLinkToken(_queryToken);
                if (!validMagicLinkToken) return Page();

                var baseServiceUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

                Data = await _mediator.Send(new GetEnquirerViewAllResponsesQuery(enquiryId, baseServiceUrl));
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
            string tokenValue;

            try
            {
                tokenValue = _aesEncrypt.Decrypt(token);
            }
            catch
            {
                var parsedToken = ParseTokenFromQueryString();

                tokenValue = _aesEncrypt.Decrypt(parsedToken);

                _queryToken = parsedToken;
            }

            var splitTokenValue = tokenValue.Split('&', StringSplitOptions.RemoveEmptyEntries);

            var splitEnquiryPart = splitTokenValue[0].Split('=', StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(splitEnquiryPart[1], out var enquiryId))
            {
                return enquiryId;
            }

            return default;
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
            var isValidMagicLinkToken = await _mediator.Send(new IsValidMagicLinkTokenQuery(token,
                nameof(MagicLinkType.EnquirerViewAllResponses)));

            if (!isValidMagicLinkToken)
            {
                AddErrorMessage(InvalidTokenErrorMessage);

                return false;
            }

            return true;
        }

        private string ParseTokenFromQueryString()
        {
            var queryString = Request.QueryString.Value;
            var tokens = queryString!.Split(new char[] { '=' }, 2);
            var tokenValue = tokens[1];
            return tokenValue;
        }
    }
}