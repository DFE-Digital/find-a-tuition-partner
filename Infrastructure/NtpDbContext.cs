using Application;
using Domain;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class NtpDbContext : DbContext, INtpDbContext, IDataProtectionKeyContext
{
    public NtpDbContext(DbContextOptions<NtpDbContext> options) : base(options)
    {
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
    public DbSet<EmailLog> EmailLog { get; set; } = null!;
    public DbSet<EmailLogHistory> EmailLogHistory { get; set; } = null!;
    public DbSet<EmailNotifyResponseLog> EmailNotifyResponseLog { get; set; } = null!;
    public DbSet<EmailPersonalisationLog> EmailPersonalisationLog { get; set; } = null!;
    public DbSet<EmailStatus> EmailStatus { get; set; } = null!;
    public DbSet<EmailTriggerActivation> EmailTriggerActivation { get; set; } = null!;
    public DbSet<EnquirerNotInterestedReason> EnquirerNotInterestedReasons { get; set; } = null!;
    public DbSet<Enquiry> Enquiries { get; set; } = null!;
    public DbSet<EnquiryResponse> EnquiryResponses { get; set; } = null!;
    public DbSet<EnquiryResponseStatus> EnquiryResponseStatus { get; set; } = null!;
    public DbSet<EstablishmentStatus> EstablishmentStatus { get; set; } = null!;
    public DbSet<EstablishmentTypeGroup> EstablishmentTypeGroup { get; set; } = null!;
    public DbSet<KeyStage> KeyStage { get; set; } = null!;
    public DbSet<KeyStageSubjectEnquiry> KeyStageSubjectsEnquiry { get; set; } = null!;
    public DbSet<LocalAuthority> LocalAuthority { get; set; } = null!;
    public DbSet<LocalAuthorityDistrictCoverage> LocalAuthorityDistrictCoverage { get; set; } = null!;
    public DbSet<LocalAuthorityDistrict> LocalAuthorityDistricts { get; set; } = null!;
    public DbSet<MagicLink> MagicLinks { get; set; } = null!;
    public DbSet<OrganisationType> OrganisationType { get; set; } = null!;
    public DbSet<PhaseOfEducation> PhaseOfEducation { get; set; } = null!;
    public DbSet<Price> Prices { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<ScheduledProcessingInfo> ScheduledProcessingInfo { get; set; } = null!;
    public DbSet<School> Schools { get; set; } = null!;
    public DbSet<SubjectCoverage> SubjectCoverage { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<TuitionPartner> TuitionPartners { get; set; } = null!;
    public DbSet<TuitionPartnerEnquiry> TuitionPartnersEnquiry { get; set; } = null!;
    public DbSet<TuitionSetting> TuitionSettings { get; set; } = null!;


    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
    }
}