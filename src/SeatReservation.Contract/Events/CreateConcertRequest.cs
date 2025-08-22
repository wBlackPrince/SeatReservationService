namespace SeatReservationService.Contract;

public record CreateConcertRequest(
    Guid VenueId,
    string Name,
    DateTime EventDate,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    string Description,
    string Performer);