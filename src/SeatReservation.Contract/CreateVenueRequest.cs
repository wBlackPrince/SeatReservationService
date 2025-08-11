namespace SeatReservationService.Contract;

public record CreateVenueRequest(
    string Prefix,
    string Name, 
    int maxSeatsCount,
    IEnumerable<CreateSeatRequest> seats);