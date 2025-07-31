using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservationDomain.Reservation;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class ReservationConfiguration: IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        
        builder.HasKey(v => v.Id).HasName("pk_resevations");
        
        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new ReservationId(id));
        
        // builder
        //     .HasMany(r => r.ReservedSeats)
        //     .WithOne(rs => rs.Reservation)
        //     .HasForeignKey("reservation_id")
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);;
    }
}