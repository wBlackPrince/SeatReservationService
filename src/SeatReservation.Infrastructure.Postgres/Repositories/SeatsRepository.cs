using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Seats;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class SeatsRepository : ISeatsRepository
{
    ReservationServiceDbContext _dbContext;
    ILogger<SeatsRepository> _logger;

    public SeatsRepository(
        ReservationServiceDbContext dbContext,
        ILogger<SeatsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Seat>> GetByIds(
        IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Seats
            .Where(s => seatIds.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }
}