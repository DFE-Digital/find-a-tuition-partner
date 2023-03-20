using Application.Common.Models.Enquiry.Respond;

namespace UI.Extensions;

public static class EnquiryResponseModelExtensions
{
    public static void EnquiryResponseParseSessionValues(this EnquiryResponseModel data, string key, string value)
    {
        switch (key)
        {
            case var k when k.Equals(StringConstants.LocalAuthorityDistrict, StringComparison.OrdinalIgnoreCase):
                data.LocalAuthorityDistrict = value;
                break;

            case var k when Equals(StringConstants.EnquiryTutoringLogistics, StringComparison.OrdinalIgnoreCase):
                data.EnquiryTutoringLogistics = value;
                break;

            case var k when Equals(StringConstants.EnquiryResponseTutoringLogistics, StringComparison.OrdinalIgnoreCase):
                data.TutoringLogisticsText = value;
                break;

            case var k when Equals(StringConstants.EnquiryKeyStageSubjects, StringComparison.OrdinalIgnoreCase):
                data.EnquiryKeyStageSubjects =
                    value.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                break;

            case var k when Equals(StringConstants.EnquiryResponseKeyStageAndSubjectsText, StringComparison.OrdinalIgnoreCase):
                data.KeyStageAndSubjectsText = value;
                break;

            case var k when Equals(StringConstants.EnquiryTuitionType, StringComparison.OrdinalIgnoreCase):
                data.EnquiryTuitionType = value;
                break;

            case var k when Equals(StringConstants.EnquiryResponseTuitionTypeText, StringComparison.OrdinalIgnoreCase):
                data.TuitionTypeText = value;
                break;

            case var k when Equals(StringConstants.EnquirySENDRequirements, StringComparison.OrdinalIgnoreCase):
                data.EnquirySENDRequirements = value;
                break;

            case var k when Equals(StringConstants.EnquiryResponseSENDRequirements, StringComparison.OrdinalIgnoreCase):
                data.SENDRequirementsText = value;
                break;

            case var k when Equals(StringConstants.EnquiryAdditionalInformation, StringComparison.OrdinalIgnoreCase):
                data.EnquiryAdditionalInformation = value;
                break;

            case var k when Equals(StringConstants.EnquiryResponseAdditionalInformation, StringComparison.OrdinalIgnoreCase):
                data.AdditionalInformationText = value;
                break;

            case var k when Equals(StringConstants.EnquiryResponseToken, StringComparison.OrdinalIgnoreCase):
                data.Token = value;
                break;
        }
    }
}