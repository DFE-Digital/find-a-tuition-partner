namespace Application.Common.Interfaces;

public interface IProcessEmailsService
{
    Task ProcessAllEmails();
    Task SendEmail(string clientReference);
}