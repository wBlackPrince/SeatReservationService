namespace SeatReservationService.Contract;

public record UpdateVenueNameRequest(Guid VenueId, string Name);