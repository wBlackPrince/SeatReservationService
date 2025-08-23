using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Reservations;

public interface IReservationsRepository
{
    Task<Result<Guid, Error>> Add(Reservation reservation, CancellationToken cancellationToken);

    Task<bool> AnySeatsAlreadyReserved(
        Guid eventId,
        IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken);

    Task<Result<Guid, Error>> Delete(ReservationId reservationId, CancellationToken cancellationToken);

    Task<int> GetReservedSeatsCount(Guid eventId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}