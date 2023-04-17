namespace Domain
{
    public class EmailLogHistory //TODO - test - will only store previous states, not the current one?
                                 //TODO - Trigger OK?
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessFromDate { get; set; }
        public DateTime? LastEmailSendAttemptDate { get; set; }

        public int EmailStatusId { get; set; }

        public int EmailLogId { get; set; }
        public EmailLog EmailLog { get; set; } = null!;
    }
}
