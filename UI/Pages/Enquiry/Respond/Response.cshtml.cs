using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond
{
    public class Response : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IEncrypt _aesEncrypt;
        private readonly ISessionService _sessionService;

        private const string InvalidTokenErrorMessage = "Invalid token provided in the URl.";

        private const string InvalidUrlErrorMessage = "Invalid Url";

        public Response(IMediator mediator, IEncrypt aesEncrypt, ISessionService sessionService)
        {
            _mediator = mediator;
            _aesEncrypt = aesEncrypt;
            _sessionService = sessionService;
        }

        [BindProperty] public EnquiryResponseModel Data { get; set; } = new();

        [ViewData] public string? ErrorMessage { get; set; }

        string _queryToken = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            /*
            TODO:
            Once confirmed update the error content and logic if:
                No token supplied; 
                Invalid URL/token supplied (possibly same message as previous error);
                Token has expired; - The expiry date is stored in db, but not inspected on this page at the moment, so is not being used.
                The enquiry has already been answered (I assume error message is shown for MVP rather than a read only version of the answers supplied?) - how to deal with this?  Possibly after validated the magic link is OK ensure that the response answers are empty?
            Ensure that the input fields are not shown on page if the above happens.
            Ensure that the above happens on all magic link pages - this authorisation kind of logic might apply to all these response pages?
            Update the magic links for TPs to be 7 days, confirm what to set for the enquiry as initial expiry date
            */

            _queryToken = Request.Query["token"].ToString();

            if (AddInValidUrlErrorMessage(_queryToken)) return Page();

            try
            {
                if (!IsParsedTokenValues(_queryToken)) return Page();

                var getMagicLinkToken = await GetMagicLinkToken(_queryToken);
                if (getMagicLinkToken == null) return Page();
            }
            catch
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return Page();
            }

            Data.BaseServiceUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            var sessionValues = await _sessionService.RetrieveDataAsync();

            if (sessionValues != null)
            {
                foreach (var sessionValue in sessionValues)
                {
                    ParseSessionValue(sessionValue.Key, sessionValue.Value);
                }
            }
            else
            {
                var enquiryData = await _mediator.Send(new
                    GetEnquirerViewAllResponsesQuery(Data.EnquiryId, Data.BaseServiceUrl));

                if (enquiryData != null)
                {
                    Data.LocalAuthorityDistrict = enquiryData.LocalAuthorityDistrict!;
                    Data.EnquiryKeyStageSubjects = enquiryData.KeyStageSubjects;
                    Data.EnquiryTuitionType = enquiryData.TuitionTypeName;
                    Data.EnquiryTutoringLogistics = enquiryData.TutoringLogistics;
                    Data.EnquirySENDRequirements = enquiryData.SENDRequirements;
                    Data.AdditionalInformationText = enquiryData.AdditionalInformation;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _queryToken = Request.Query["token"].ToString();

            if (AddInValidUrlErrorMessage(_queryToken)) return Page();

            try
            {
                if (!IsParsedTokenValues(_queryToken)) return Page();

                var getMagicLinkToken = await GetMagicLinkToken(_queryToken);
                if (getMagicLinkToken == null) return Page();

                Data.BaseServiceUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

                await _sessionService.AddOrUpdateDataAsync(new Dictionary<string, string>()
                {
                    { StringConstants.LocalAuthorityDistrict, Data.LocalAuthorityDistrict! },
                    { StringConstants.EnquiryResponseTutoringLogistics, Data.TutoringLogisticsText! },
                    { StringConstants.EnquiryResponseKeyStageAndSubjectsText, Data.KeyStageAndSubjectsText! },
                    { StringConstants.EnquiryResponseTuitionTypeText, Data.TuitionTypeText! },
                    { StringConstants.EnquiryResponseSENDRequirements, Data.SENDRequirementsText ?? string.Empty },
                    {
                        StringConstants.EnquiryResponseAdditionalInformation,
                        Data.AdditionalInformationText ?? string.Empty
                    },
                    { StringConstants.EnquiryResponseToken, Data.Token! },
                    { StringConstants.EnquiryKeyStageSubjects, string.Join(Environment.NewLine, Data.EnquiryKeyStageSubjects!) },
                    { StringConstants.EnquiryTuitionType, Data.EnquiryTuitionType! },
                    { StringConstants.EnquiryTutoringLogistics, Data.EnquiryTutoringLogistics! },
                    { StringConstants.EnquirySENDRequirements, Data.EnquirySENDRequirements ?? string.Empty },
                    { StringConstants.EnquiryAdditionalInformation, Data.AdditionalInformationText ?? string.Empty },
                });

                return RedirectToPage(nameof(CheckYourAnswers));
            }
            catch
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return Page();
            }
        }

        private bool IsParsedTokenValues(string token)
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

                if (string.IsNullOrWhiteSpace(tokenValue))
                {
                    AddErrorMessage(InvalidUrlErrorMessage);
                    return false;
                };
            }

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

            Data.Token = _queryToken;

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

        private void ParseSessionValue(string key, string value)
        {
            switch (key)
            {
                case var k when k.Contains(StringConstants.LocalAuthorityDistrict):
                    Data.LocalAuthorityDistrict = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryTutoringLogistics):
                    Data.EnquiryTutoringLogistics = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryResponseTutoringLogistics):
                    Data.TutoringLogisticsText = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryKeyStageSubjects):
                    Data.EnquiryKeyStageSubjects = value.Split(Environment.NewLine).ToList();
                    break;

                case var k when k.Contains(StringConstants.EnquiryResponseKeyStageAndSubjectsText):
                    Data.KeyStageAndSubjectsText = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryTuitionType):
                    Data.EnquiryTuitionType = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryResponseTuitionTypeText):
                    Data.TuitionTypeText = value;
                    break;

                case var k when k.Contains(StringConstants.EnquirySENDRequirements):
                    Data.EnquirySENDRequirements = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryResponseSENDRequirements):
                    Data.SENDRequirementsText = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryAdditionalInformation):
                    Data.EnquiryAdditionalInformation = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryResponseAdditionalInformation):
                    Data.AdditionalInformationText = value;
                    break;

                case var k when k.Contains(StringConstants.EnquiryResponseToken):
                    Data.Token = value;
                    break;
            }
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