namespace Domain
{
    public class EmailNotifyResponseLog
    {
        public int Id { get; set; }
        public string? NotifyId { get; set; } //see https://docs.notifications.service.gov.uk/net.html#send-an-email-response
        public string? Reference { get; set; }
        public string? Uri { get; set; }
        public string? TemplateId { get; set; }
        public string? TemplateUri { get; set; }
        public int? TemplateVersion { get; set; }
        public string? EmailResponseContentFrom { get; set; }
        public string? EmailResponseContentBody { get; set; }
        public string? EmailResponseContentSubject { get; set; }

        public string? ExceptionCode { get; set; }
        public string? ExceptionMessage { get; set; }

        public int EmailLogId { get; set; }
        public EmailLog EmailLog { get; set; } = null!;
    }
}
