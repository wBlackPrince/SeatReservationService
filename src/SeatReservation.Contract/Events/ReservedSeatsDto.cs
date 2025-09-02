namespace SeatReservationService.Contract.Events;

public record ReservedSeatsDto
{
    public Guid Id { get; init; }
    
    public int RowNumber { get; init; }
    
    public int SeatNumber { get; init; }
    
    public Guid VenueId { get; init; }
    
    public bool IsAvailable { get; init; }
}