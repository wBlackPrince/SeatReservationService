using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using SeatReservation.Infrastructure.Postgres.Database;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class NpgsqlVenuesRepository: IVenuesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<EfCoreVenuesRepository> _logger;
    
    public NpgsqlVenuesRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<EfCoreVenuesRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Error>> Add(Venue venue, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var transaction = connection.BeginTransaction();

        try
        {
            const string venueInsertSql = """
                                          INSERT INTO venues (id, prefix, name, max_seats_count)
                                          VALUES (@Id, @Prefix, @Name, @MaxSeatsCount)
                                          """;

            var venueInsertParams = new
            {
                Id = venue.Id.Value,
                Prefix = venue.Name.Prefix,
                Name = venue.Name.Name,
                MaxSeatsCount = venue.MaxSeatsCount
            };

            await connection.ExecuteAsync(
                venueInsertSql, 
                venueInsertParams);


            if (!venue.Seats.Any())
            {
                return venue.Id.Value;
            }
        
            const string insertIntoSeats = """
                                           INSERT INTO seats (id, venue_id, row_number, seat_number)
                                           VALUES (@Id, @VenueId, @RowNumber, @SeatNumber)
                                           """;

            var seatsInsertParams = venue.Seats.Select(s
                => new { Id = s.Id.Value, VenueId = venue.Id.Value, SeatNumber = s.SeatNumber, RowNumber = s.RowNumber });
        
            await connection.ExecuteAsync(
                insertIntoSeats, 
                seatsInsertParams);
            
            transaction.Commit();
        
            return venue.Id.Value;

        }
        catch(Exception ex)
        {
            transaction.Rollback();
            
            _logger.LogError(ex, "An error occured while adding new venue");

            return Error.Failure("venue.insert", "fail to insert venue");
        }
    }
    
    
    public async Task<Result<Guid, Error>> UpdateVenueName(
        VenueId venueId, 
        VenueName venueName, 
        CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var transaction = connection.BeginTransaction();

        try
        {
            const string updateNameSql = """
                                          UPDATE venues
                                          SET name = @Name
                                          WHERE id = @Id
                                          """;

            var updateNametParams = new
            {
                Id = venueId.Value,
                Name = venueName.Name,
            };

            await connection.ExecuteAsync(
                updateNameSql, 
                updateNametParams);
            
            transaction.Commit();
            
            return venueId.Value;
        }
        catch(Exception ex)
        {
            transaction.Rollback();
            
            _logger.LogError(ex, "Fail to update venue name");

            return Error.Failure("venue.update.name", "fail to update venue name");
        }
    }


    public async Task<UnitResult<Error>> UpdateVenueNameByPrefix(
        string prefix,
        VenueName venueName,
        CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        
        var transaction = connection.BeginTransaction();

        try
        {
            const string updateNameSql = """
                                         UPDATE venues
                                         SET name = @Name
                                         WHERE prefix = @Prefix
                                         """;

            var updateNameByPrefixParams = new
            {
                Prefix = prefix,
                Name = venueName.Name,
            };

            await connection.ExecuteAsync(
                updateNameSql, 
                updateNameByPrefixParams);
            
            transaction.Commit();

            return UnitResult.Success<Error>();
        }
        catch(Exception ex)
        {
            transaction.Rollback();
            
            _logger.LogError(ex, "Fail to update venue name by prefix");

            return Error.Failure("venue.update.name", "fail to update venue name by prefix");
        }
    }



    public async Task<Result<Venue, Error>> GetById(
        VenueId id,
        CancellationToken cancellationToken) => throw new NotImplementedException();
    
    
    public async Task Save() => throw new NotImplementedException();
    
    public async Task Update(Venue venue, CancellationToken cancellationToken) => throw new NotImplementedException();
    
    public async Task<IReadOnlyList<Venue>> GetByPrefix(
        string prefix,
        CancellationToken cancellationToken) => throw new NotImplementedException();
    
    public async Task<UnitResult<Error>> DeleteSeatsByVenueId(
        VenueId venueId, 
        CancellationToken cancellationToken) => throw new NotImplementedException();

    public async Task<UnitResult<Error>> AddSeats(
        IEnumerable<Seat> seats,
        CancellationToken cancellationToken) => throw new NotImplementedException();
    
    public async Task<Result<Venue, Error>> GetByIdWithSeats(
        VenueId id,
        CancellationToken cancellationToken) => throw new NotImplementedException();
}