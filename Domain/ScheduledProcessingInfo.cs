namespace Domain
{
    public class ScheduledProcessingInfo
    {
        public int Id { get; set; }
        public string ScheduleName { get; set; } = null!;
        public DateTime LastStartedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastFinishedDate { get; set; }
        public string? Status { get; set; }
    }
}
