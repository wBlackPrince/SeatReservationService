using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIds(
        IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken);
}