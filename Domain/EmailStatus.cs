namespace Domain
{
    public class EmailStatus
    {
        public int Id { get; set; }

        public string Status { get; set; } = null!;

        public string Description { get; set; } = null!;

        public bool AllowEmailSending { get; set; } //If the email should be sent (in conjunction with EmailLog.ProcessFromDate and EmailLog.FinishProcessingDate being valid)

        public bool PollForStatusUpdateIfSent { get; set; } //If we should check Notify for status updates (in conjunction with EmailLog.LastEmailSendAttemptDate and EmailLog.FinishProcessingDate being valid)

        public int? RetrySendInSeconds { get; set; } //If the sending should be retired against this status, how many seconds until retry (updates the EmailLog.ProcessFromDate)

        public ICollection<EmailLog>? EmailLogs { get; set; }
    }
}
