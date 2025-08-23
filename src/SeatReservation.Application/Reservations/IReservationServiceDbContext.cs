using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SeatReservationDomain.Event;
using SeatReservationDomain.Users;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Reservations;

public interface IReservationServiceDbContext
{
    DbSet<Venue> Venues { get; }
    DbSet<User> Users { get; }
    DbSet<Event> Events { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    ChangeTracker ChangeTracker { get; }
}