namespace Application.Common.Models.Admin;

public record ProcessedEmailsModel
{
    public int EmailsCheckStatus { get; set; } = 0;
    public int EmailsUpdatedStatus { get; set; } = 0;
    public int EmailsSent { get; set; } = 0;
    public string Outcome { get; set; } = null!;
}