namespace Domain
{
    public class EmailLog
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessFromDate { get; set; } //Used for batching, will only send the email after this date.
                                                       //If null then the email is not sent (e.g. awaiting response from enquirer before sending to TPs)
        public DateTime? LastEmailSendAttemptDate { get; set; } //This will be when NTP 1st sends the email.
                                                                //It will be updated with any retries if there have been issues.
        public DateTime FinishProcessingDate { get; set; } = DateTime.UtcNow.AddDays(2); //This is how long we want to try and send emails for
        public DateTime? LastStatusChangedDate { get; set; } //If status changes we update this

        public string EmailAddress { get; set; } = null!;
        public string EmailTemplateShortName { get; set; } = null!;
        public string ClientReferenceNumber { get; set; } = null!;
        public string? EmailAddressUsedForTesting { get; set; }

        public int EmailStatusId { get; set; }

        public EmailStatus EmailStatus { get; set; } = null!;
        public EmailNotifyResponseLog? EmailNotifyResponseLog { get; set; }
        public ICollection<EmailPersonalisationLog>? EmailPersonalisationLogs { get; set; }
        public ICollection<EmailLogHistory>? EmailLogHistories { get; set; }

        public ICollection<Enquiry>? EnquirerEnquiriesSubmitted { get; set; }
        public ICollection<TuitionPartnerEnquiry>? TuitionPartnerEnquiriesSubmitted { get; set; }
        public ICollection<EnquiryResponse>? EnquirerResponses { get; set; }
        public ICollection<EnquiryResponse>? TuitionPartnerResponses { get; set; }
        public EnquiryResponse? TuitionPartnerResponseNotInterested { get; set; }

        public EmailTriggerActivation? ThisEmailActivationTriggeredBy { get; set; }
        public ICollection<EmailTriggerActivation>? EmailsActivatedByThisEmail { get; set; }
    }
}
