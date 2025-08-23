using CSharpFunctionalExtensions;
using SeatReservation.Shared;
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
    
    private Reservation(ReservationId id, Guid eventId, Guid userId, IEnumerable<Guid> seatIds)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        
        var reservedSeats = seatIds
            .Select(seatId => new ReservationSeat( 
                new ReservationSeatId(Guid.NewGuid()), 
                this, 
                new SeatId(seatId), 
                eventId,
                CreatedAt))
            .ToList();
        _seats = reservedSeats;
    }
    
    public ReservationId Id { get; private set; }
    
    public Guid EventId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public ReservationStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public IReadOnlyList<ReservationSeat> ReservedSeats  => _seats;

    public static Reservation Create(Guid eventId, Guid userId,IEnumerable<Guid> seatIds)
    {
        Reservation reservation = new Reservation(
            new ReservationId(Guid.NewGuid()), 
            eventId, 
            userId,
            seatIds);
        
        return reservation;
    }
}