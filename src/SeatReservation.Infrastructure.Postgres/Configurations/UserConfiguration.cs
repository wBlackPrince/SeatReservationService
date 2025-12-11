using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservationDomain.Users;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(v => v.Id).HasName("pk_users");

        builder
            .Property(u => u.Id)
            .HasColumnName("id");
            
        builder.OwnsOne(u => u.Details, db =>
        {
            db.ToJson("details");

            db.OwnsMany(d => d.Socials, sb =>
            {

                sb.OwnsOne(s => s.Name, sbb =>
                {
                    sbb
                        .Property(ss => ss.Value)
                        .IsRequired()
                        .HasMaxLength(SocialNetworkName.MaxLength)
                        .HasColumnName("name");
                });
                
                sb.OwnsOne(s => s.Link, sbb =>
                {
                    sbb
                        .Property(ss => ss.Value)
                        .IsRequired()
                        .HasMaxLength(SocialNetworkLink.MaxLength)
                        .HasColumnName("link");
                });
            });

            db.OwnsOne(d => d.Description, sb =>
            {
                sb.Property(sd => sd.Value).HasColumnName("description");
            });

            db.OwnsOne(d => d.FIO, sb =>
            {
                sb.Property(sd => sd.Value).HasColumnName("fio");
            });
        });
    }
}