using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SeatReservationDomain;
using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Database;

public interface IReservationServiceDbContext
{
    DbSet<Venue> Venues { get; }
    DbSet<User> Users { get; }
    DbSet<Event> Events { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    ChangeTracker ChangeTracker { get; }
}