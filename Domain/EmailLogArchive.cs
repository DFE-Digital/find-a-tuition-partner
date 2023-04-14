namespace Domain
{
    public class EmailLogArchive //TODO - decide - will this only store previous states, not the current one?
                                 //TODO - how populate - Trigger?
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessFromDate { get; set; }
        public DateTime? LastEmailSendAttemptDate { get; set; }

        public int EmailStatusId { get; set; }

        public string? NotifySuccessId { get; set; }
        public string? NotifyExceptionCode { get; set; }
        public string? NotifyExceptionMessage { get; set; }

        public int EmailLogId { get; set; }
        public EmailLog EmailLog { get; set; } = null!;
    }
}
