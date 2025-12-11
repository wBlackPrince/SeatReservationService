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

        builder
            .Property(v => v.Id)
            .HasColumnName("id");

        builder.OwnsOne(e => e.Capacity, eb =>
        {
            eb
                .Property(ed => ed.Value)
                .HasColumnName("capacity");
        });
        
        builder.OwnsOne(e => e.Description, eb =>
        {
            eb
                .Property(ed => ed.Value)
                .HasColumnName("description");
        });
        
        builder
            .HasOne<Event>()
            .WithOne(e => e.Details)
            .HasForeignKey<EventDetails>(ed => ed.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ed => ed.Version)
            .IsRowVersion();
    }
}