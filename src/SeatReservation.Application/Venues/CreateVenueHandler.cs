using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract;

namespace SeatReservationService.Application.Venues;

public class CreateVenueHandler
{
    private readonly IReservationServiceDbContext _dbContext;
    private readonly IVenuesRepository _venuesRepository;
    
    public CreateVenueHandler(
        IReservationServiceDbContext dbContext,
        IVenuesRepository venuesRepository)
    {
        _dbContext = dbContext;
        _venuesRepository = venuesRepository;
    }
    
    /// <summary>
    /// Создает площадки со всеми местами
    /// </summary>
    public async Task<Result<Guid, Error>> Handle(
        CreateVenueRequest request, 
        CancellationToken cancellationToken)
    {
        
        var venue = Venue.Create(
            request.Prefix,
            request.Name,
            request.maxSeatsCount);

        if (venue.IsFailure)
        {
            return venue.Error;
        }
        
        List<Seat> seats =[];

        foreach (var seatRequest in request.seats)
        {
            var seat = Seat.Create(venue.Value, seatRequest.RowNumber, seatRequest.ColumnNumber);

            if (seat.IsFailure)
            {
                return seat.Error;
            }
            
            seats.Add(seat.Value);
        }
        
        
        venue.Value.AddSeats(seats);
        
        // await _dbContext.Venues.AddAsync(venue.Value, cancellationToken);
        // await _dbContext.SaveChangesAsync(cancellationToken);
        
        await _venuesRepository.Add(venue.Value, cancellationToken);
        
        
        return venue.Value.Id.Value;
    }
}