using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class NtpDbContext : DbContext, INtpDbContext
{
    public NtpDbContext(DbContextOptions<NtpDbContext> options) : base(options)
    {
    }

    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<LocalAuthority> LocalAuthorities { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<TuitionPartner> TuitionPartners { get; set; } = null!;
    public DbSet<TuitionPartnerLocation> TuitionPartnerLocations { get; set; } = null!;
    public DbSet<TuitionType> TuitionTypes { get; set; } = null!;
    public DbSet<TutorType> TutorTypes { get; set; } = null!;
    public DbSet<UserSearch> UserSearches { get; set; } = null!;
    public DbSet<UserSession> UserSessions { get; set; } = null!;

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
    }
}