namespace Domain
{
    public class EmailLog
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessFromDate { get; set; } //Use for batching in future?  Also if null then not to try sending (e.g. awaiting response from enquirer before sending to TPs)
        public DateTime? LastEmailSendAttemptDate { get; set; } //When 1st send email or any retries
        public DateTime FinishProcessingDate { get; set; } = DateTime.UtcNow.AddDays(7); //How long we want to try and send emails for - e.g. 7 days for TP reponse?
        public DateTime? LastStatusChangedDate { get; set; } //If status changes we update this

        public string EmailAddress { get; set; } = null!;
        public string EmailTemplateId { get; set; } = null!;
        public string ClientReferenceNumber { get; set; } = null!; //TODO - client ref needs to be updated to inc environment so is unique

        public int EmailStatusId { get; set; }

        public EmailStatus EmailStatus { get; set; } = null!;
        public EmailNotifyResponseLog? EmailNotifyResponseLog { get; set; }
        public ICollection<EmailPersonalisationLog>? EmailPersonalisationLogs { get; set; }
        public ICollection<EmailLogHistory>? EmailLogHistories { get; set; }

    }
}
