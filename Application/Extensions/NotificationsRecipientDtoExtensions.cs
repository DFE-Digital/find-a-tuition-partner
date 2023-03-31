using Application.Common.DTO;
using Domain.Enums;

namespace Application.Extensions;

public static class NotificationsRecipientDtoExtensions
{
    public static void AddDefaultEnquiryDetails(this NotificationsRecipientDto notificationsRecipient,
        string enquiryRef, string baseUrl, EmailTemplateType emailTemplateType, DateTime? dateTime, string? tpName = null)
    {
        notificationsRecipient.Personalisation.AddDefaultEnquiryPersonalisation(enquiryRef, baseUrl, dateTime);

        var tpSeoUrl = string.IsNullOrWhiteSpace(tpName) ? string.Empty : $"-{tpName.ToSeoUrl()}";
        notificationsRecipient.ClientReference = $"{enquiryRef}-{emailTemplateType.DisplayName()}{tpSeoUrl}";
        notificationsRecipient.ClientReferenceIfAmalgamate = $"{enquiryRef}-{emailTemplateType.DisplayName()}";
    }
}
