namespace Domain
{
    public class EmailPersonalisationLog
    {
        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;

        public int EmailLogId { get; set; }
        public EmailLog EmailLog { get; set; } = null!;
    }
}
