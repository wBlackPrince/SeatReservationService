using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Venue;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class ReservationSeatConfiguration: IEntityTypeConfiguration<ReservationSeat>
{
    public void Configure(EntityTypeBuilder<ReservationSeat> builder)
    {
        builder.ToTable("reservation_seats");
        
        builder.HasKey(v => v.Id).HasName("pk_reservation_seats");
        
        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new ReservationSeatId(id));
        
        builder
            .HasOne(rs => rs.Reservation)
            .WithMany(r => r.ReservedSeats)
            .HasForeignKey("reservation_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(rs => rs.SeatId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(rs => rs.SeatId)
            .IsRequired()
            .HasColumnName("seat_id");
        
        builder
            .Property(rs => rs.EventId)
            .IsRequired()
            .HasColumnName("event_id");
        
        builder
            .HasIndex(rs => new {rs.EventId, rs.SeatId})
            .IsUnique();
    }
}