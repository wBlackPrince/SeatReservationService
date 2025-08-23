namespace SeatReservationService.Contract.venues;

public record UpdateVenueNameRequest(Guid VenueId, string Name);