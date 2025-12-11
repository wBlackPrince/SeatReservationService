using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Reservation;


public class Reservation
{
    public List<ReservationSeat> _seats;

    // Ef Core
    private Reservation()
    {
        
    }
    
    private Reservation(
        Guid id,
        Guid eventId,
        Guid userId,
        IEnumerable<Guid> seatIds)
    {
        Id = id;
        EventId = eventId;
        UserId = userId;
        
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        
        var reservedSeats = seatIds
            .Select(seatId => new ReservationSeat( 
                Guid.NewGuid(), 
                this, 
                seatId, 
                eventId,
                CreatedAt))
            .ToList();
        _seats = reservedSeats;
    }
    
    public Guid Id { get; private set; }
    
    public Guid EventId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public ReservationStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public IReadOnlyList<ReservationSeat> ReservedSeats  => _seats;

    public static Result<Reservation, Error> Create(
        Guid eventId,
        Guid userId,
        IEnumerable<Guid> seatIds)
    {
        if (eventId == Guid.Empty)
        {
            return Error.Validation(
                "reservation.eventId", 
                "Event ID cannot be empty");
        }

        if (userId == Guid.Empty)
        {
            return Error.Validation(
                "reservation.userId",
                "User ID cannot be empty");
        }

        var seatIdsList = seatIds?.ToList() ?? [];

        if (seatIdsList.Count == 0)
        {
            return Error.Validation(
                "reservation.seats", 
                "At least one seat must be selected");
        }

        if (seatIdsList.Any(seatId => seatId == Guid.Empty))
        {
            return Error.Validation(
                "reservation.seats",
                "Seat IDs cannot be empty");
        }

        return new Reservation(
            Guid.NewGuid(), 
            eventId, 
            userId,
            seatIdsList);
    }
}