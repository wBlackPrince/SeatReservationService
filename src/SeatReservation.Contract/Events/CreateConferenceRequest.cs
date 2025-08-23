namespace SeatReservationService.Contract.Events;

public record CreateConferenceRequest(
    Guid VenueId,
    string Name,
    DateTime EventDate,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    string Description,
    string Speaker,
    string Topic);