using Application.Common.Models.Enquiry;

namespace UI.Extensions;

public static class EnquiryResponseModelExtensions
{
    public static void EnquiryResponseParseSessionValues(this EnquiryResponseBaseModel data, string key, string value)
    {
        switch (key)
        {
            case var k when k.Equals(SessionKeyConstants.LocalAuthorityDistrict, StringComparison.OrdinalIgnoreCase):
                data.LocalAuthorityDistrict = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryTutoringLogistics, StringComparison.OrdinalIgnoreCase):
                data.EnquiryTutoringLogisticsDisplayModel.TutoringLogistics = value;
                data.EnquiryTutoringLogisticsDisplayModel.TutoringLogisticsDetailsModel = value.ToTutoringLogisticsDetailsModel();
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

            case var k when k.Equals(SessionKeyConstants.EnquiryTuitionSetting, StringComparison.OrdinalIgnoreCase):
                data.EnquiryTuitionSetting = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryResponseTuitionSettingText, StringComparison.OrdinalIgnoreCase):
                data.TuitionSettingText = value;
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
        }
    }
}