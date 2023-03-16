using Application.Common.Models.Enquiry.Respond;

namespace UI.Extensions;

public static class EnquiryResponseModelExtensions
{
    public static void EnquiryResponseParseSessionValues(this EnquiryResponseModel data, string key, string value)
    {
        switch (key)
        {
            case var k when k.Contains(StringConstants.LocalAuthorityDistrict):
                data.LocalAuthorityDistrict = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryTutoringLogistics):
                data.EnquiryTutoringLogistics = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseTutoringLogistics):
                data.TutoringLogisticsText = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryKeyStageSubjects):
                data.EnquiryKeyStageSubjects =
                    value.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseKeyStageAndSubjectsText):
                data.KeyStageAndSubjectsText = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryTuitionType):
                data.EnquiryTuitionType = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseTuitionTypeText):
                data.TuitionTypeText = value;
                break;

            case var k when k.Contains(StringConstants.EnquirySENDRequirements):
                data.EnquirySENDRequirements = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseSENDRequirements):
                data.SENDRequirementsText = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryAdditionalInformation):
                data.EnquiryAdditionalInformation = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseAdditionalInformation):
                data.AdditionalInformationText = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseToken):
                data.Token = value;
                break;
        }
    }
}