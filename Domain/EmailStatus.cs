namespace Domain
{
    public class EmailStatus
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!;

        public string Description { get; set; } = null!;

        public bool AllowEmailSending { get; set; }

        public bool PollForStatusUpdateIfSent { get; set; }

        public int? RetrySendInSeconds { get; set; }

        public ICollection<EmailLog>? EmailLogs { get; set; }
    }
}
