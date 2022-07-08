using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasIndex(e => e.Name);

        builder.HasData(
            new Region { Id = Regions.Id.NorthEast, Code = "E12000001", Name = "North East" },
            new Region { Id = Regions.Id.NorthWest, Code = "E12000002", Name = "North West" },
            new Region { Id = Regions.Id.YorkshireandTheHumber, Code = "E12000003", Name = "Yorkshire and The Humber" },
            new Region { Id = Regions.Id.EastMidlands, Code = "E12000004", Name = "East Midlands" }, 
            new Region { Id = Regions.Id.WestMidlands, Code = "E12000005", Name = "West Midlands" },
            new Region { Id = Regions.Id.EastofEngland, Code = "E12000006", Name = "East of England" },
            new Region { Id = Regions.Id.London, Code = "E12000007", Name = "London" },
            new Region { Id = Regions.Id.SouthEast, Code = "E12000008", Name = "South East" },
            new Region { Id = Regions.Id.SouthWest, Code = "E12000009", Name = "South West" }
        );
    }
}