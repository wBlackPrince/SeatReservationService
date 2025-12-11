using System.Data;
using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract.venues;

namespace SeatReservationService.Application.Venues.Commands;

public class UpdateVenueSeatsHandler
{
    private readonly IVenuesRepository _venuesRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateVenueSeatsHandler(
        IVenuesRepository venuesRepository,
        ITransactionManager transactionManager)
    {
        _venuesRepository = venuesRepository;
        _transactionManager = transactionManager;
    }
    
    
    public async Task<Result<Guid, Error>> Handle(
        UpdateVenueSeatsRequest request,
        CancellationToken cancellationToken)
    {
        var venueId = request.VenueId;
        
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        if (transactionScopeResult.IsFailure)
        {
            return transactionScopeResult.Error;
        }
        
        using var transactionScope = transactionScopeResult.Value;
        
        await _venuesRepository.DeleteSeatsByVenueId(venueId, cancellationToken);
        
        var venue = await _venuesRepository.GetById(venueId, cancellationToken);
        if (venue.IsFailure)
        {
            transactionScope.Rollback();
            return venue.Error;
        }
        
        List<Seat> seats = new List<Seat>();  
        
        foreach (var seatRequest in request.Seats)  
        {            
            var seat = Seat.Create(  
                venueId,                
                seatRequest.rowNumber,   
                seatRequest.seatNumber);  
  
            if (seat.IsFailure)  
            {                
                return seat.Error;  
            }     

            seats.Add(seat.Value);
        }  
        
        venue.Value.UpdateSeats(seats);

        await _transactionManager.SaveChangesAsync(cancellationToken);
        
        var commitedResult =  transactionScope.Commit();

        if (commitedResult.IsFailure)
        {
            return commitedResult.Error;
        }
        
        return request.VenueId;
    }
}