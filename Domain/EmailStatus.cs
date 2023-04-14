namespace Domain
{
    public class EmailStatus
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!; //Values = cr   ated / sending / delivered / permanent-failure / temporary-failure / technical-failure ->
                                                    //TODO get and understand the notify status - when happen.  What additional atatus needed - e.g. waiting to be triggered, delayed for batch reasons etc
        public bool Retry { get; set; } //Will depend on status?

        public ICollection<EmailLog>? EmailLogs { get; set; }
    }
}
