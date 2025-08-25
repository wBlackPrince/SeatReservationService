using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using SeatReservationDomain;
using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Users;
using SeatReservationDomain.Venue;
using SeatReservationService.Application;
using SeatReservationService.Application.Reservations;

namespace SeatReservation.Infrastructure.Postgres;

public class ReservationServiceDbContext: DbContext, IReservationServiceDbContext
{
    private readonly string _connectionString;

    static ReservationServiceDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    
    public ReservationServiceDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationServiceDbContext).Assembly);
    }
    
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<User> Users => Set<User>();
    
    public DbSet<Event> Events => Set<Event>();

    public DbSet<Seat> Seats => Set<Seat>();
    
    public DbSet<Reservation> Reservations => Set<Reservation>();
    
    public DbSet<ReservationSeat> ReservationSeats => Set<ReservationSeat>();



    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}

