using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Infrastructure.Postgres.Converters;
using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class EventConfiguration: IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");
        
        builder.HasKey(v => v.Id).HasName("pk_events");
        
        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new EventId(id));

        builder.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(e => e.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(e => e.VenueId).HasColumnName("venue_id");

        builder.Property(e => e.Type).HasConversion<string>().HasColumnName("type");

        builder.Property(e => e.Info)
            .HasConversion(new EventInfoConverter())
            .HasColumnName("info");
    }
}