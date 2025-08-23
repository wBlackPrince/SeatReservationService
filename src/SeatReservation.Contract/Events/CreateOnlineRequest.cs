namespace SeatReservationService.Contract.Events;

public record CreateOnlineRequest(
    Guid VenueId,
    string Name,
    DateTime EventDate,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    string Description,
    string Url);