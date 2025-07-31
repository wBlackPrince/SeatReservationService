using SeatReservationDomain.Venue;

namespace SeatReservationDomain.Reservation;

public record ReservationId(Guid Value);

public class Reservation
{
    public List<ReservationSeat> _seats;

    // Ef Core
    private Reservation()
    {
        
    }
    
    public Reservation(ReservationId id, IEnumerable<Guid> seatIds)
    {
        Id = id;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        
        var reservedSeats = seatIds
            .Select(seatId => new ReservationSeat( new ReservationSeatId(Guid.NewGuid()), this, new SeatId(Guid.NewGuid()), CreatedAt))
            .ToList();
        _seats = reservedSeats;
    }
    
    public ReservationId Id { get; private set; }
    
    public Guid EventId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public ReservationStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public IReadOnlyList<ReservationSeat> ReservedSeats  => _seats;
}