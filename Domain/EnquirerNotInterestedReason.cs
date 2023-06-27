namespace Domain
{
    public class EnquirerNotInterestedReason
    {
        public int Id { get; set; }

        public string Description { get; set; } = null!;

        public bool CollectAdditionalInfoIfSelected { get; set; }

        public int OrderBy { get; set; }

        public bool IsActive { get; set; }

        public ICollection<EnquiryResponse>? EnquiryResponses { get; set; }
    }
}
