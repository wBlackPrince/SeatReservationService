using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Event;
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

    public static Result<Reservation, Error> Create(
        EventId eventId,
        Guid userId,
        IEnumerable<Guid> seatIds)
    {
        if (eventId.Value == Guid.Empty)
        {
            return Error.Validation("reservation.eventId", "Event ID cannot be empty");
        }

        if (userId == Guid.Empty)
        {
            return Error.Validation("reservation.userId", "User ID cannot be empty");
        }

        var seatIdsList = seatIds?.ToList() ?? [];

        if (seatIdsList.Count == 0)
        {
            return Error.Validation("reservation.seats", "At least one seat must be selected");
        }

        if (seatIdsList.Any(seatId => seatId == Guid.Empty))
        {
            return Error.Validation("reservation.seats", "Seat IDs cannot be empty");
        }

        return new Reservation(new ReservationId(Guid.NewGuid()), eventId.Value, userId, seatIdsList);
    }
}