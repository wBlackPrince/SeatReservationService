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

        builder
            .Property(e => e.Id)
            .HasColumnName("event_id");
        
        builder.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(e => e.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.Info)
            .HasConversion(new EventInfoConverter())
            .HasColumnName("info");

        builder.OwnsOne(v => v.Name, vb =>
        {
            vb.Property(vn => vn.Value).HasColumnName("name");
        });

        builder.OwnsOne(e => e.Dates, eb =>
        {
            eb.Property(ed => ed.EventDate).HasColumnName("event_date");
            
            eb.Property(ed => ed.StartDate).HasColumnName("start_date");
            
            eb.Property(ed => ed.EndDate).HasColumnName("end_date");
        });
        
        builder.Property(e => e.Status)
            .HasConversion(
                o => o.ToString(),
                event_status => (EventStatus)Enum.Parse(typeof(EventStatus), event_status))
            .HasColumnName("status");
        
        builder.Property(e => e.Type)
            .HasConversion(
                o => o.ToString(),
                event_type => (EventType)Enum.Parse(typeof(EventType), event_type))
            .HasColumnName("type");
    }
}