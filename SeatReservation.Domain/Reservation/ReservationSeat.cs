namespace SeatReservationDomain.Reservation;

public class ReservationSeat
{
    public ReservationSeat(Guid id, Reservation reservation, Guid seatId, DateTime reservationDate)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservationAt = reservationDate;
    }
    
    
    public Guid Id { get; private set; }
    public Reservation Reservation { get; private set; }
    public Guid SeatId { get; private set; }
    
    public DateTime ReservationAt { get; private set; }
}