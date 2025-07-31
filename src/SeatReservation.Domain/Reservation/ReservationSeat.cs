using SeatReservationDomain.Venue;

namespace SeatReservationDomain.Reservation;


public record ReservationSeatId(Guid Value);

public class ReservationSeat
{
    // Ef Core
    private ReservationSeat()
    {
    }
    
    public ReservationSeat(
        ReservationSeatId id, 
        Reservation reservation, 
        SeatId seatId, 
        DateTime reservationDate)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservationAt = reservationDate;
    }
    
    
    public ReservationSeatId Id { get; private set; }
    public Reservation Reservation { get; private set; }
    public SeatId SeatId { get; private set; }
    
    public DateTime ReservationAt { get; private set; }
}