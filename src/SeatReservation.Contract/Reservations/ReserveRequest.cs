namespace SeatReservationService.Contract.Reservations;

public record ReserveRequest(Guid EventId, Guid UserId, IEnumerable<Guid> Seats);