using SeatReservationDomain.Event;

namespace SeatReservationService.Application.Database;

public interface IReadDbContext
{
    IQueryable<Event> EventsRead { get; }
}