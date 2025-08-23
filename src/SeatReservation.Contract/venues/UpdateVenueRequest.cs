namespace SeatReservationService.Contract.venues;

public record UpdateVenueRequest(Guid VenueId, string Name, string Prefix, int MaxSeatsLimit);