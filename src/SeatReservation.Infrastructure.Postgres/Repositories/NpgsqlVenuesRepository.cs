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
}