using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Venue;
using EventId = SeatReservationDomain.Event.EventId;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class ReservationsRepository : IReservationsRepository
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<VenuesRepository> _logger;

    public ReservationsRepository(
        ReservationServiceDbContext dbContext,
        ILogger<VenuesRepository> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Add(Reservation reservation, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return reservation.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Error.Failure("reservation", "reservation.failure");
        }
    }

    public async Task<bool> AnySeatsAlreadyReserved(
        Guid eventId,
        IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken)
    {
        var hasReservedSeats = _dbContext.Reservations
            .Where(r => r.EventId == eventId)
            .Where(r => r.ReservedSeats.Any(rs => seatIds.Contains(rs.SeatId)))
            .AnyAsync(cancellationToken);

        return hasReservedSeats.Result;
    }
}