using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Venue;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public interface IReservationsRepository
{
    Task<Result<Guid, Error>> Add(Reservation reservation, CancellationToken cancellationToken);

    Task<bool> AnySeatsAlreadyReserved(
        Guid eventId,
        IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken);
}