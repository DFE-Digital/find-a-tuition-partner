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
    public DbSet<LocalAuthorityDistrict> LocalAuthorityDistricts { get; set; } = null!;
    public DbSet<LocalAuthorityDistrictCoverage> LocalAuthorityDistrictCoverage { get; set; } = null!;
    public DbSet<Price> Prices { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<TuitionPartner> TuitionPartners { get; set; } = null!;
    public DbSet<TuitionType> TuitionTypes { get; set; } = null!;
    public DbSet<SubjectCoverage> SubjectCoverage { get; set; } = null!;

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
    }
}