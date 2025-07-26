using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Venue;

public class Seat
{
    private Seat(Guid id, int rowNumber, int seatNumber)
    {
        Id = id;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }
    
    public Guid Id { get; set; }
    
    public int RowNumber { get; private set; }
    
    public int SeatNumber { get; private set; }

    public static Result<Seat, Error> Create(int rowNumber, int seatNumber)
    {
        if (rowNumber < 0 || seatNumber < 0)
        {
            return Error.Validation("seats.rowNumber", "Номера места не могут быть отрицательными!");
        }
        return new Seat(Guid.NewGuid(), rowNumber, seatNumber);
    }
}