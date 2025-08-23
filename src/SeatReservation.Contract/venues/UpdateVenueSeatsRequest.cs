using SeatReservationService.Contract.Seats;

namespace SeatReservationService.Contract.venues;

public record UpdateVenueSeatsRequest(Guid VenueId, IEnumerable<UpdateSeatRequest> Seats);