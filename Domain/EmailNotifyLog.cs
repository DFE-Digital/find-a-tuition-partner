namespace Domain
{
    public class EmailNotifyLog
    {
        public int Id { get; set; }
        public string NotifyId { get; set; } = null!; //see https://docs.notifications.service.gov.uk/net.html#send-an-email-response
        public string Reference { get; set; } = null!;
        public string Uri { get; set; } = null!;
        public string TemplateId { get; set; } = null!;
        public string TemplateUri { get; set; } = null!;
        public int TemplateVersion { get; set; }
        public string EmailResponseContentFrom { get; set; } = null!;
        public string EmailResponseContentBody { get; set; } = null!;
        public string EmailResponseContentSubject { get; set; } = null!;

        public EmailLog EmailLog { get; set; } = null!;
    }
}
