using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain.Reservation;
using SeatReservationService.Application.Reservations;

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
            
            return reservation.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Error.Failure("reservation", "reservation.failure");
        }
    }

    public async Task<bool> AnySeatsAlreadyReserved(
        Guid eventId,
        IEnumerable<Guid> seatIds,
        CancellationToken cancellationToken)
    {
        var hasReservedSeats = _dbContext.Reservations
            .Where(r => r.EventId == eventId)
            .Where(r => r.ReservedSeats.Any(rs => seatIds.Contains(rs.SeatId)))
            .AnyAsync(cancellationToken);

        return hasReservedSeats.Result;
    }

    public async Task<Result<Guid, Error>> Delete(Guid reservationId, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations.FirstOrDefaultAsync(
            r => r.Id == reservationId, 
            cancellationToken);

        if (reservation is null)
        {
            return Error.Failure("reservation.delete", "no reservation with this id was found");
        }

        _dbContext.Reservations.Remove(reservation);
        
        return reservation.Id;
    }

    public async Task<int> GetReservedSeatsCount(Guid eventId, CancellationToken cancellationToken)
    {
        // await _dbContext.Database.ExecuteSqlAsync(
        //     $"SELECT capacity from event_details WHERE event_id = {eventId} FOR UPDATE",
        //     cancellationToken);
        
        return await _dbContext.Reservations
            .Where(r => r.EventId == eventId)
            .Where(r => 
                (r.Status == ReservationStatus.Confirmed) || 
                (r.Status == ReservationStatus.Pending))
            .SelectMany(r => r.ReservedSeats)
            .CountAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}