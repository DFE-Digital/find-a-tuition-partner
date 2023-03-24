using Application.Constants;

namespace Application.Extensions;

public static class DictionaryExtensions
{
    private const string EnquiryRefKeyKey = "enquiry_ref_number";
    private const string EnquiryDateTimeKey = "date_time";
    private const string EnquiryContactUsKey = "contact_us_link";

    public static Dictionary<string, dynamic> AddDefaultEnquiryPersonalisation(this Dictionary<string, dynamic> personalisation,
        string enquiryRef, DateTime dateTime, string baseUrl)
    {
        if (personalisation != null)
        {
            personalisation.Add(EnquiryRefKeyKey, enquiryRef);
            personalisation.Add(EnquiryDateTimeKey, dateTime.ToString(StringConstants.DateFormatGDS));
            personalisation.Add(EnquiryContactUsKey, $"{baseUrl}/contact-us");
        }

        return personalisation ?? new Dictionary<string, dynamic>();
    }
}
