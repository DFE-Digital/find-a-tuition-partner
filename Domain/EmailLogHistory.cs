namespace Domain
{
    public class EmailLogHistory
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
