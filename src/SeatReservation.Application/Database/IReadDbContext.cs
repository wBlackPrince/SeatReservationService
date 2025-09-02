using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Users;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Database;

public interface IReadDbContext
{
    IQueryable<Event> EventsRead { get; }
    
    IQueryable<Seat> SeatsRead { get; }
    
    IQueryable<Venue> VenuesRead { get; }
    
    IQueryable<User> UsersRead { get; }
    
    IQueryable<Reservation> ReservationsRead { get; }
    
    IQueryable<ReservationSeat> ReservationSeatsRead { get; }
}