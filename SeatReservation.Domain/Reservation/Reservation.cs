namespace SeatReservationDomain.Reservation;

public class Reservation
{
    public List<ReservationSeat> _seats;
    
    public Reservation(Guid id, IEnumerable<Guid> seatIds)
    {
        Id = id;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        
        var reservednSeats = seatIds
            .Select(seatId => new ReservationSeat(Guid.NewGuid(), this, seatId, CreatedAt))
            .ToList();
        _seats = reservednSeats;
    }
    
    public Guid Id { get; private set; }
    
    public Guid EventId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public ReservationStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public IReadOnlyList<ReservationSeat> Seats  => _seats;
}