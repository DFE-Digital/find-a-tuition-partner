using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build
{
    public class SubmittedConfirmation : PageModel
    {
        public SubmittedConfirmationModel Data { get; set; } = new();

        public void OnGet(SubmittedConfirmationModel data)
        {
            var tpMagicLinksFromQueryString = Request.Query["TuitionPartnerMagicLinks"];
            if (tpMagicLinksFromQueryString.Count > 0)
            {
                var tpMagicLinks =
                    (from tp in tpMagicLinksFromQueryString
                     where !string.IsNullOrWhiteSpace(tp)
                     select tp.Trim('[', ']').Split(',')
                        into split
                     let tuitionPartnerSeoUrl = split[0].Substring(split[0].IndexOf('=') + 2)
                     let magicLinkToken = split[1].Substring(split[1].IndexOf('=') + 2)
                     let email =
                         split[2].Substring(split[2].IndexOf('=') + 2,
                             split[2].IndexOf('}') - split[2].IndexOf('=') - 2)
                     select new TuitionPartnerMagicLinkModel
                     {
                         TuitionPartnerSeoUrl = tuitionPartnerSeoUrl,
                         MagicLinkToken = magicLinkToken,
                         Email = email
                     }).ToList();

                data.TuitionPartnerMagicLinks = tpMagicLinks;
            }

            Data = data;
        }
    }
}
