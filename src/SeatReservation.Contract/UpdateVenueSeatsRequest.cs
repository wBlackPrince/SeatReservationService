namespace SeatReservationService.Contract;

public record UpdateVenueSeatsRequest(Guid VenueId, IEnumerable<UpdateSeatRequest> Seats);