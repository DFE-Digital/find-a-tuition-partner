using Domain.Enums;

namespace Application.Common.Interfaces;

public interface INotificationsClientService
{
    Task SendEmailAsync(List<string> recipients, EmailTemplateType emailTemplateType, Dictionary<string, dynamic> personalisation);
}