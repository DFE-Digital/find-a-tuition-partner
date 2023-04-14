namespace Domain
{
    public class EmailLog
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastProcessedAt { get; set; } //We update this if re-run the failed job?
        public DateTime? NextProcessDate { get; set; } //Use for batching in future?  Also if null then not to try sending (e.g. awaiting response from enquirere before sending to TPs)
        public DateTime FinishProcessingAt { get; set; } = DateTime.UtcNow.AddDays(7); //How long we want to try and send emails for - e.g. 7 days for TP reponse?
        
        public string EmailAddress { get; set; } = null!;
        public string EmailTemplateId { get; set; } = null!;
        public string ClientReferenceNumber { get; set; } = null!; //Named as ClientReferenceNumber
                                                                    //TODO - client ref needs to be updated to inc envirnment so is unique
        
        public string NotifyExceptionCode { get; set; } = null!; //TODO - decide if should be in EmailErrorLog?  But if so need to clear out when success and how store in EmailLogArchive?
        public string NotifyExceptionMessage { get; set; } = null!;


        public int EmailStatusId { get; set; }
        public int? EmailNotifyLogId { get; set; }

        public EmailStatus EmailStatus { get; set; } = null!;
        public EmailNotifyLog? EmailNotifyLog { get; set; }
        public ICollection<EmailPersonalisationLog>? EmailPersonalisationLogs { get; set; }
        public ICollection<EmailLogArchive>? EmailLogArchives { get; set; }

    }
}
