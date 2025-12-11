using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Venue;


public class Seat
{
    // EF Core
    private Seat()
    {
        
    }
    private Seat(Guid id, Venue venue, int rowNumber, int seatNumber)
    {
        Id = id;
        Venue = venue;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }
    
    private Seat(Guid id, Guid venueId, int rowNumber, int seatNumber)
    {
        Id = id;
        VenueId = venueId;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }
    
    public Guid Id { get; set; }

    public Guid VenueId { get; private set; }

    public Venue Venue { get; private set; } = null!;
    
    public int RowNumber { get; private set; }
    
    public int SeatNumber { get; private set; }

    public static Result<Seat, Error> Create(Venue venue, int rowNumber, int seatNumber)
    {
        if (rowNumber < 0 || seatNumber < 0)
        {
            return Error.Validation("seats.rowNumber", "Номера места не могут быть отрицательными!");
        }
        return new Seat(Guid.NewGuid(), venue, rowNumber, seatNumber);
    }
    
    public static Result<Seat, Error> Create(Guid venueId, int rowNumber, int seatNumber)
    {
        if (rowNumber < 0 || seatNumber < 0)
        {
            return Error.Validation("seats.rowNumber", "Номера места не могут быть отрицательными!");
        }
        return new Seat(Guid.NewGuid(), venueId, rowNumber, seatNumber);
    }
}