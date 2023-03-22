using Application.Common.DTO;
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
                if (!IsParseTokenTypeFromToken(_queryToken)) return Page();

                var getMagicLinkToken = await GetMagicLinkToken(_queryToken);

                var baseServiceUrl = Request.GetBaseServiceUrl();

                if (getMagicLinkToken != null)
                {
                    Data = await _mediator.Send(new GetEnquirerViewAllResponsesQuery(getMagicLinkToken.EnquiryId!.Value, baseServiceUrl));
                    HttpContext.AddLadNameToAnalytics(Data.LocalAuthorityDistrict);
                    HttpContext.AddEnquirySupportReferenceNumberToAnalytics(Data.SupportReferenceNumber);
                }
            }
            catch
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return Page();
            }

            return Page();
        }

        private bool IsParseTokenTypeFromToken(string token)
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

                _queryToken = parsedToken;

                if (string.IsNullOrWhiteSpace(tokenValue)) return false;

            }

            var splitTokenValue = tokenValue.Split('&', StringSplitOptions.RemoveEmptyEntries);

            if (!splitTokenValue.Any()) return false;
            var splitTokenTypePart = splitTokenValue[0].Split('=', StringSplitOptions.RemoveEmptyEntries);

            var tokenType = splitTokenTypePart[1];

            if (!string.IsNullOrWhiteSpace(tokenType) &&
                tokenType != nameof(MagicLinkType.EnquirerViewAllResponses))
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return false;
            }

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
            var getMagicLinkTokenQuery = await _mediator.Send(new GetMagicLinkTokenQuery(token,
                nameof(MagicLinkType.EnquirerViewAllResponses)));

            if (getMagicLinkTokenQuery != null) return getMagicLinkTokenQuery;
            AddErrorMessage(InvalidTokenErrorMessage);
            return null;

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