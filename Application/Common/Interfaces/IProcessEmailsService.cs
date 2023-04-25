using Application.Common.Models.Admin;
using Domain;

namespace Application.Common.Interfaces;

public interface IProcessEmailsService
{
    Task<ProcessedEmailsModel> ProcessAllEmailsAsync();
    Task<int> SendEmailsAsync(int[] emailLogIds);
    Task<int> SendEmailAsync(int emailLogId);
    string? GetEmailAddressUsedForTesting(string? emailToBeUsedIfTestingEnabled = null);
    EmailLog? MergeEmailForTesting(List<EmailLog> emailLogs, List<string> personalisationPropertiesToMerge);
}