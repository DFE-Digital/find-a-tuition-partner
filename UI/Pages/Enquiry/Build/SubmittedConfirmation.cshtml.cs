using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build
{
    public class SubmittedConfirmation : PageModel
    {
        public SubmittedConfirmationModel Data { get; set; } = new();

        public void OnGet(SubmittedConfirmationModel data)
        {
            foreach (var tpLink in Request.Query["TuitionPartnerMagicLinks"])
            {
                if (!string.IsNullOrWhiteSpace(tpLink))
                {
                    var split = tpLink.Trim('[', ']').Split(',');
                    data.TuitionPartnerMagicLinks!.Add(split[0].Trim(), split[1].Trim());
                }
            }

            Data = data;
        }
    }
}
