namespace Domain
{
    public class EnquiryResponseStatus
    {
        public int Id { get; set; }

        public string Status { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int OrderBy { get; set; }

        public ICollection<EnquiryResponse>? EnquiryResponses { get; set; }
    }
}
