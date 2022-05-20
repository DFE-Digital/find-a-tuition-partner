using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations
{
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasIndex(e => e.Code);

            builder.HasData(
                new Region { Id = 1, Code = "E12000001", Name = "North East" },
                new Region { Id = 2, Code = "E12000002", Name = "North West" },
                new Region { Id = 3, Code = "E12000003", Name = "Yorkshire and The Humber" },
                new Region { Id = 4, Code = "E12000004", Name = "East Midlands" }, 
                new Region { Id = 5, Code = "E12000005", Name = "West Midlands" },
                new Region { Id = 6, Code = "E12000006", Name = "East of England" },
                new Region { Id = 7, Code = "E12000007", Name = "London" },
                new Region { Id = 8, Code = "E12000008", Name = "South East" },
                new Region { Id = 9, Code = "E12000009", Name = "South West" }
                );
        }
    }
}
