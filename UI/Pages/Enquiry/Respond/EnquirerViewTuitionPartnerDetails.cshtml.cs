using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond
{
    public class EnquirerViewTuitionPartnerDetails : PageModel
    {
        private const string InvalidTokenErrorMessage = "Invalid token provided in the URl.";

        private const string InvalidUrlErrorMessage = "Invalid Url";

        private readonly IMediator _mediator;
        private readonly IEncrypt _aesEncrypt;
        [BindProperty] public EnquirerViewTuitionPartnerDetailsModel Data { get; set; } = new();
        [ViewData] public string ErrorMessage { get; set; } = string.Empty;

        string _queryToken = string.Empty;

        public EnquirerViewTuitionPartnerDetails(IMediator mediator, IEncrypt aesEncrypt)
        {
            _mediator = mediator;
            _aesEncrypt = aesEncrypt;
        }


        public async Task<IActionResult> OnGetAsync()
        {

            _queryToken = Request.Query["token"].ToString();

            if (AddInValidUrlErrorMessage(_queryToken)) return Page();

            try
            {
                var (enquiryId, tuitionPartnerId) = GetTokenValues(_queryToken);

                var getMagicLinkToken = await GetMagicLinkToken(_queryToken);
                if (getMagicLinkToken == null) return Page();

                var getEnquirerViewTuitionPartnerDetailsQuery = new GetEnquirerViewTuitionPartnerDetailsQuery(enquiryId, tuitionPartnerId);

                var data = await _mediator.Send(getEnquirerViewTuitionPartnerDetailsQuery);

                if (data != null)
                {
                    Data = data with { EnquirerViewResponseToken = _queryToken };
                }
            }
            catch
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return Page();
            }

            return Page();
        }

        //TODO - review repeated code with the other response pages that have magic links - to be looked at as part of https://dfedigital.atlassian.net/browse/NTP-1041
        private (int enquiryId, int tuitionPartnerId) GetTokenValues(string token)
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
            var enquiryId = int.Parse(splitEnquiryPart[1]);

            var splitTuitionPartnerPart = splitTokenValue[1].Split('=', StringSplitOptions.RemoveEmptyEntries);
            var tuitionPartnerId = int.Parse(splitTuitionPartnerPart[1]);

            return (enquiryId, tuitionPartnerId);
        }

        private void AddErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;

            ModelState.AddModelError("ErrorMessage", ErrorMessage);
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
                nameof(MagicLinkType.EnquirerViewResponse)));

            if (getMagicLinkTokenQuery == null)
            {
                AddErrorMessage(InvalidTokenErrorMessage);

                return null;
            }

            return getMagicLinkTokenQuery;
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