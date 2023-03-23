using Application.Common.Models.Enquiry.Respond;

namespace UI.Extensions;

public static class EnquiryResponseModelExtensions
{
    public static void EnquiryResponseParseSessionValues(this EnquiryResponseModel data, string key, string value)
    {
        switch (key)
        {
            case var k when k.Equals(SessionKeyConstants.LocalAuthorityDistrict, StringComparison.OrdinalIgnoreCase):
                data.LocalAuthorityDistrict = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryTutoringLogistics, StringComparison.OrdinalIgnoreCase):
                data.EnquiryTutoringLogistics = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryResponseTutoringLogistics, StringComparison.OrdinalIgnoreCase):
                data.TutoringLogisticsText = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryKeyStageSubjects, StringComparison.OrdinalIgnoreCase):
                data.EnquiryKeyStageSubjects =
                    value.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryResponseKeyStageAndSubjectsText, StringComparison.OrdinalIgnoreCase):
                data.KeyStageAndSubjectsText = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryTuitionType, StringComparison.OrdinalIgnoreCase):
                data.EnquiryTuitionType = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryResponseTuitionTypeText, StringComparison.OrdinalIgnoreCase):
                data.TuitionTypeText = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquirySENDRequirements, StringComparison.OrdinalIgnoreCase):
                data.EnquirySENDRequirements = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryResponseSENDRequirements, StringComparison.OrdinalIgnoreCase):
                data.SENDRequirementsText = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryAdditionalInformation, StringComparison.OrdinalIgnoreCase):
                data.EnquiryAdditionalInformation = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryResponseAdditionalInformation, StringComparison.OrdinalIgnoreCase):
                data.AdditionalInformationText = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryResponseToken, StringComparison.InvariantCultureIgnoreCase):
                data.Token = value;
                break;
        }
    }
}