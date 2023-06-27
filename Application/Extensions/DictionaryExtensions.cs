using Application.Constants;

namespace Application.Extensions;

public static class DictionaryExtensions
{
    private const string EnquiryRefKeyKey = "enquiry_ref_number";
    private const string EnquiryDateTimeKey = "date_time";
    private const string EnquiryContactUsKey = "contact_us_link";

    public static Dictionary<string, dynamic> AddDefaultEnquiryPersonalisation(this Dictionary<string, dynamic> personalisation,
        string? enquiryRef, string baseUrl, DateTime? dateTime = null)
    {
        personalisation ??= new Dictionary<string, dynamic>();

        if (!string.IsNullOrWhiteSpace(enquiryRef))
        {
            personalisation.Add(EnquiryRefKeyKey, enquiryRef);
        }

        if (dateTime != null)
        {
            personalisation.Add(EnquiryDateTimeKey, dateTime.Value.ToLocalDateTime().ToString(StringConstants.DateTimeFormatGDS));
        }

        personalisation = personalisation.AddDefaultEmailPersonalisation(baseUrl);

        return personalisation;
    }

    public static Dictionary<string, dynamic> AddDefaultEmailPersonalisation(this Dictionary<string, dynamic> personalisation,
        string baseUrl)
    {
        personalisation ??= new Dictionary<string, dynamic>();

        personalisation.Add(EnquiryContactUsKey, $"{baseUrl}/contact-us");

        return personalisation;
    }
}
