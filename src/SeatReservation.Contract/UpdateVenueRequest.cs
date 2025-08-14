namespace SeatReservationService.Contract;

public record UpdateVenueRequest(Guid VenueId, string Name, string Prefix, int MaxSeatsLimit);