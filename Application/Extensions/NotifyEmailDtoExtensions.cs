using Application.Common.DTO;
using Domain.Enums;

namespace Application.Extensions;

//TODO - delete this?
public static class NotifyEmailDtoExtensions
{
    public static void AddDefaultEnquiryDetails(this NotifyEmailDto notifyEmail, string clientRefPrefix,
        string enquiryRef, string baseUrl, EmailTemplateType emailTemplateType, DateTime? dateTime, string? tpName = null)
    {
        notifyEmail.Personalisation.AddDefaultEnquiryPersonalisation(enquiryRef, baseUrl, dateTime);

        notifyEmail.ClientReference = enquiryRef.CreateNotifyClientReference(clientRefPrefix, emailTemplateType, tpName);
    }
}
