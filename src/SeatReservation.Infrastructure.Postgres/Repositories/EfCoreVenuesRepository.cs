using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class EfCoreVenuesRepository: IVenuesRepository
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<EfCoreVenuesRepository> _logger;

    public EfCoreVenuesRepository(
        ReservationServiceDbContext dbContext,
        ILogger<EfCoreVenuesRepository> logger
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
        
            return venue.Id.Value;
        }
        catch (Exception e)
        {
            return Error.Failure("venue.insert", "fail to insert venue");
        }
    }

    public async Task<Result<Guid, Error>> UpdateVenueName(
        VenueId venueId, 
        VenueName venueName, 
        CancellationToken cancellationToken)
    {
        await _dbContext.Venues
            .Where(v => v.Id == venueId)
            .ExecuteUpdateAsync(
            setter => 
                setter
                    .SetProperty(v => v.Name.Name, venueName.Name)
                    .SetProperty(v =>v.MaxSeatsCount, 100), cancellationToken);

        return venueId.Value;
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
        VenueId id,
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
        VenueId id,
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

    

    public async Task Save()
    {
        var entires = _dbContext.ChangeTracker.Entries();
        
        await _dbContext.SaveChangesAsync();
    }
    
    
    public async Task<UnitResult<Error>> DeleteSeatsByVenueId(
        VenueId venueId, 
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