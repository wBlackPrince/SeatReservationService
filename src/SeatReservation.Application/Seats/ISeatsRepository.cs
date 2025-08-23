using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Seats;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIds(
        IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken);
}