using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservationDomain.Event;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class EventDetailsConfiguration: IEntityTypeConfiguration<EventDetails>
{
    public void Configure(EntityTypeBuilder<EventDetails> builder)
    {
        builder.ToTable("event_details");
        
        builder.HasKey(v => v.Id).HasName("pk_event_details");
        
        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new EventDetailsId(id));
        
        builder
            .HasOne<Event>()
            .WithOne(e => e.Details)
            .HasForeignKey<EventDetails>(ed => ed.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}