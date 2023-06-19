namespace Domain
{
    public class EmailTriggerActivation
    {
        public int Id { get; set; }
        public int EmailLogId { get; set; }
        public int ActivateEmailLogId { get; set; }

        public EmailLog EmailLog { get; set; } = null!;
        public EmailLog ActivateEmailLog { get; set; } = null!;
    }
}
