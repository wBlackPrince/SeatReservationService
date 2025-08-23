using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Shared;
using SeatReservationDomain;
using SeatReservationDomain.Users;
using SeatReservationDomain.Venue;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(v => v.Id).HasName("pk_users");
        
        builder.Property(v => v.Id)
            .HasColumnName("id");

        
        
        builder.OwnsOne(u => u.Details, db =>
        {
            db.ToJson("details");

            db.OwnsMany(d => d.Socials, sb =>
            {
                sb.Property(s => s.Link)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.Length500)
                    .HasColumnName("link");
            
                sb.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(LengthConstants.Length500)
                    .HasColumnName("name");
            });

            db.Property(d => d.Description)
                .HasColumnName("description");
            
            db.Property(d => d.FIO)
                .HasColumnName("foi");
        });
    }
}