using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Venues;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class VenuesRepository: IVenuesRepository
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<VenuesRepository> _logger;

    public VenuesRepository(
        ReservationServiceDbContext dbContext,
        ILogger<VenuesRepository> logger
        )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Add(Venue venue, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Venues.AddAsync(venue, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        
            return venue.Id;
        }
        catch (Exception e)
        {
            return Error.Failure("venue.insert", "fail to insert venue");
        }
    }

    public async Task<Result<Guid, Error>> UpdateVenueName(
        Guid venueId, 
        VenueName venueName, 
        CancellationToken cancellationToken)
    {
        _dbContext.Database.ExecuteSqlAsync(
            $"UPDATE venues SET Name = {venueName.Name} WHERE Id = {venueId}", cancellationToken);
        _dbContext.Database.ExecuteSqlRawAsync(
            "UPDATE venues SET Name = @Name WHERE Id = @Value", 
            new NpgsqlParameter("@Name", venueName.Name),
            new NpgsqlParameter("@Id", venueId));
        
        
        // await _dbContext.Venues
        //     .Where(v => v.Id == venueId)
        //     .ExecuteUpdateAsync(
        //     setter => 
        //         setter
        //             .SetProperty(v => v.Name.Name, venueName.Name)
        //             .SetProperty(v =>v.MaxSeatsCount, 100), cancellationToken);

        return venueId;
    }
    
    public async Task<UnitResult<Error>> UpdateVenueNameByPrefix(
        string prefix, 
        VenueName venueName, 
        CancellationToken cancellationToken)
    {
        await _dbContext.Venues
            .Where(v => v.Name.Prefix == prefix)
            .ExecuteUpdateAsync(
                setter => 
                    setter
                        .SetProperty(v => v.Name.Name, venueName.Name)
                        .SetProperty(v =>v.MaxSeatsCount, 100), cancellationToken);

        return UnitResult.Success<Error>();
    }


    public async Task Update(Venue venue, CancellationToken cancellationToken)
    {
        _dbContext.Venues.Update(venue);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    
    public async Task<Result<Venue, Error>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var venue = await _dbContext.Venues
            .Include(v => v.Seats)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (venue is null)
        {
            return Error.Failure("update.venue.name", "Venue not found");
        }

        return venue;
    }
    
    public async Task<Result<Venue, Error>> GetByIdWithSeats(
        Guid id,
        CancellationToken cancellationToken)
    {
        var venue = await _dbContext.Venues
            .Include(v => v.Seats)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (venue is null)
        {
            return Error.Failure("update.venue.name", "Venue not found");
        }

        return venue;
    }
    
    
    public async Task<IReadOnlyList<Venue>> GetByPrefix(
        string prefix,
        CancellationToken cancellationToken)
    {
        var venues = await _dbContext.Venues
            .Where(v => v.Name.Prefix.StartsWith(prefix))
            .ToListAsync(cancellationToken);

        var entries = _dbContext.ChangeTracker.Entries();

        return venues;
    }
    
    
    public async Task<UnitResult<Error>> DeleteSeatsByVenueId(
        Guid venueId, 
        CancellationToken cancellationToken)
    {
        await _dbContext.Seats
            .Where(s => s.Venue.Id == venueId)
            .ExecuteDeleteAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
    
    public async Task<UnitResult<Error>> AddSeats(
        IEnumerable<Seat> seats, 
        CancellationToken cancellationToken)
    {
        await _dbContext.Seats
            .AddRangeAsync(seats, cancellationToken);

        return UnitResult.Success<Error>();
    }
}