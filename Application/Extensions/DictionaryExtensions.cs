using Application.Constants;

namespace Application.Extensions;

public static class DictionaryExtensions
{
    private const string EnquiryRefKeyKey = "enquiry_ref_number";
    private const string EnquiryDateTimeKey = "date_time";
    private const string EnquiryContactUsKey = "contact_us_link";

    public static Dictionary<string, dynamic> AddDefaultEnquiryPersonalisation(this Dictionary<string, dynamic> personalisation,
        string? enquiryRef, string baseUrl, DateTime? dateTime)
    {
        personalisation ??= new Dictionary<string, dynamic>();

        if (!string.IsNullOrWhiteSpace(enquiryRef))
        {
            personalisation.Add(EnquiryRefKeyKey, enquiryRef);
        }

        personalisation.Add(EnquiryContactUsKey, $"{baseUrl}/contact-us");

        if (dateTime != null)
        {
            personalisation.Add(EnquiryDateTimeKey, dateTime.Value.ToLocalDateTime().ToString(StringConstants.DateFormatGDS));
        }

        return personalisation;
    }
}
