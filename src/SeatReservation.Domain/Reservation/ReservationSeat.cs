using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;

namespace SeatReservationDomain.Reservation;

public class ReservationSeat
{
    // Ef Core
    private ReservationSeat()
    {
    }
    
    public ReservationSeat(
        Guid id, 
        Reservation reservation, 
        Guid seatId, 
        Guid eventId,
        DateTime reservationDate)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        EventId = eventId;
        ReservationAt = reservationDate;
    }
    
    
    public Guid Id { get; private set; }
    public Reservation Reservation { get; private set; }
    public Guid SeatId { get; private set; }
    
    public Guid EventId { get; private set; }
    
    public DateTime ReservationAt { get; private set; }
}