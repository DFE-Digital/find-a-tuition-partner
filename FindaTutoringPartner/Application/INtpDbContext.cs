using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application;

public interface INtpDbContext
{
    DbSet<TuitionPartner> TuitionPartners { get; set; }

    Task<int> SaveChangesAsync();
}