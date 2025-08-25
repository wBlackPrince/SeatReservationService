using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Seats;

public interface ISeatsRepository
{
    Task<IReadOnlyList<Seat>> GetByIds(
        IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Seat>> GetAvailableSeats(
        VenueId venueId,
        EventId eventId,
        int? rowNumber,
        CancellationToken cancellationToken);
}