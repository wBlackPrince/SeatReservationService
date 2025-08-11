using CSharpFunctionalExtensions;
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
}