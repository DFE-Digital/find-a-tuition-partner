using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EstablishmentStatusConfiguration : IEntityTypeConfiguration<EstablishmentStatus>
{
    public void Configure(EntityTypeBuilder<EstablishmentStatus> builder)
    {
        builder.HasIndex(e => e.Name);
        builder.HasData(
            new EstablishmentStatus { Id = (int)EstablishmentsStatus.Open, Name = "Open" },
            new EstablishmentStatus { Id = (int)EstablishmentsStatus.Closed, Name = "Closed" },
            new EstablishmentStatus { Id = (int)EstablishmentsStatus.OpenButProposedToClose, Name = "Open but proposed to close" },
            new EstablishmentStatus { Id = (int)EstablishmentsStatus.ProposedToOpen, Name = "Proposed to open" }
        );
    }
}