using SeatReservationService.Contract.Seats;

namespace SeatReservationService.Contract.venues;

public record CreateVenueRequest(
    string Prefix,
    string Name, 
    int maxSeatsCount,
    IEnumerable<CreateSeatRequest> seats);