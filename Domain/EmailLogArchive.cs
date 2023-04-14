namespace Domain
{
    public class EmailLogArchive //TODO - decide - will this only store previous states, not the current one?
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int EmailStatusId { get; set; }

        public string? NotifySuccessId { get; set; } = null!;
        public string NotifyExceptionCode { get; set; } = null!;
        public string NotifyExceptionMessage { get; set; } = null!;

        public int EmailLogId { get; set; }
        public EmailLog EmailLog { get; set; } = null!;
    }
}
