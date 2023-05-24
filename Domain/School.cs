namespace Domain
{
    public class School
    {
        public int Id { get; set; }
        public int Urn { get; set; }
        public string EstablishmentName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Postcode { get; set; } = null!;
        public int EstablishmentNumber { get; set; }
        public int? Ukprn { get; set; } = null;
        public bool IsActive { get; set; }
        public int EstablishmentTypeGroupId { get; set; }
        public EstablishmentTypeGroup EstablishmentTypeGroup { get; set; } = null!;
        public int EstablishmentStatusId { get; set; }
        public EstablishmentStatus EstablishmentStatus { get; set; } = null!;
        public int PhaseOfEducationId { get; set; }
        public PhaseOfEducation PhaseOfEducation { get; set; } = null!;
        public int LocalAuthorityId { get; set; }
        public LocalAuthority LocalAuthority { get; set; } = null!;
        public int LocalAuthorityDistrictId { get; set; }
        public LocalAuthorityDistrict LocalAuthorityDistrict { get; set; } = null!;
        public ICollection<Enquiry>? Enquiries { get; set; }
    }
}

