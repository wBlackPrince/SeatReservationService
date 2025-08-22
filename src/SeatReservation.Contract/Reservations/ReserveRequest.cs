namespace SeatReservationService.Contract;

public record ReserveRequest(Guid EventId, Guid UserId, IEnumerable<Guid> Seats);