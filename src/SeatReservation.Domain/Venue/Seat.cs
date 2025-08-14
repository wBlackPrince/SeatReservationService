using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Venue;


public record SeatId(Guid Value);

public class Seat
{
    // EF Core
    private Seat()
    {
        
    }
    private Seat(SeatId id, Venue venue, int rowNumber, int seatNumber)
    {
        Id = id;
        Venue = venue;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }
    
    private Seat(SeatId id, VenueId venueId, int rowNumber, int seatNumber)
    {
        Id = id;
        VenueId = venueId;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }
    
    public SeatId Id { get; set; }

    public VenueId VenueId { get; private set; } = null!;

    public Venue Venue { get; private set; } = null!;
    
    public int RowNumber { get; private set; }
    
    public int SeatNumber { get; private set; }

    public static Result<Seat, Error> Create(Venue venue, int rowNumber, int seatNumber)
    {
        if (rowNumber < 0 || seatNumber < 0)
        {
            return Error.Validation("seats.rowNumber", "Номера места не могут быть отрицательными!");
        }
        return new Seat(new SeatId(Guid.NewGuid()), venue, rowNumber, seatNumber);
    }
    
    public static Result<Seat, Error> Create(VenueId venueId, int rowNumber, int seatNumber)
    {
        if (rowNumber < 0 || seatNumber < 0)
        {
            return Error.Validation("seats.rowNumber", "Номера места не могут быть отрицательными!");
        }
        return new Seat(new SeatId(Guid.NewGuid()), venueId, rowNumber, seatNumber);
    }
}