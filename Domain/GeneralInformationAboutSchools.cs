namespace Domain
{
    public class GeneralInformationAboutSchools
    {
        public int Id { get; set; }
        public int Urn { get; set; }
        public string EstablishmentName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public EstablishmentTypeGroup EstablishmentTypeGroup { get; set; } = null!;
        public EstablishmentStatus EstablishmentStatus { get; set; } = null!;
        public PhaseOfEducation PhaseOfEducation { get; set; } = null!;
        public LocalAuthority LocalEducationAuthority { get; set; } = null!;
        public LocalAuthorityDistrict LocalAuthorityDistrict { get; set; } = null!;
    }
}

