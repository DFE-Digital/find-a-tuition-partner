namespace Domain
{
    public class EmailTriggerActivation
    {
        public int Id { get; set; } //TODO - https://stackoverflow.com/questions/55624174/how-to-bridge-a-table-on-itself-using-ef-core-code-first
        public int EmailLogId { get; set; }
        public int ActivateEmailLogId { get; set; }

        public EmailLog EmailLog { get; set; } = null!;
        public EmailLog ActivateEmailLog { get; set; } = null!;
    }
}
