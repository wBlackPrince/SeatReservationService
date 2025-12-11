using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Seats;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIds(
        IEnumerable<Guid> seatIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Seat>> GetAvailableSeats(
        Guid venueId,
        Guid eventId,
        int? rowNumber,
        CancellationToken cancellationToken);
}