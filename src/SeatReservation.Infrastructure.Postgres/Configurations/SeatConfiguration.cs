using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservationDomain.Venue;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class SeatConfiguration: IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("seats");
        
        builder.HasKey(v => v.Id).HasName("pk_seats");
        
        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new SeatId(id));
    }
}