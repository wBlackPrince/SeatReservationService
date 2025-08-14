using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class VenueConfiguration: IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");
        
        builder.HasKey(v => v.Id).HasName("pk_venues");
        
        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new VenueId(id))
            .HasColumnName("id");

        builder.Property(v => v.MaxSeatsCount)
            .HasColumnName("max_seats_count");

        builder.ComplexProperty(v => v.Name, nb =>
        {
            nb.Property(v => v.Prefix)
                .IsRequired()
                .HasMaxLength(LengthConstants.Length50)
                .HasColumnName("prefix");
            
            nb.Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(LengthConstants.Length50)
                .HasColumnName("name");
        });

        builder
            .HasMany(v => v.Seats)
            .WithOne(s => s.Venue)
            .HasForeignKey(s => s.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}