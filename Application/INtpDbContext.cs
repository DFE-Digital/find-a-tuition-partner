using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application;

public interface INtpDbContext
{
    DbSet<LocalAuthorityDistrict> LocalAuthorityDistricts { get; set; }
    DbSet<Region> Regions { get; set; }
    DbSet<Subject> Subjects { get; set; }
    DbSet<TuitionPartner> TuitionPartners { get; set; }
    DbSet<TuitionType> TuitionTypes { get; set; }
    DbSet<TutorType> TutorTypes { get; set; }
    Task<int> SaveChangesAsync();
}