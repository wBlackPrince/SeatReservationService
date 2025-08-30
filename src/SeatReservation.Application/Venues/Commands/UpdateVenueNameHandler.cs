using System.Data;
using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract;
using SeatReservationService.Contract.venues;

namespace SeatReservationService.Application.Venues;

public class UpdateVenueNameHandler
{
    private readonly IVenuesRepository _venuesRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateVenueNameHandler(
        IVenuesRepository venuesRepository,
        ITransactionManager transactionManager)
    {
        _venuesRepository = venuesRepository;
        _transactionManager = transactionManager;
    }
    
    
    public async Task<Result<Guid, Error>> Handle(
        UpdateVenueNameRequest nameRequest,
        CancellationToken cancellationToken)
    {
        var venueId = new VenueId(nameRequest.VenueId);
        
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);
        
        var transactionScope = transactionScopeResult.Value;

        var (_, isFailure, venue, error) = await _venuesRepository.GetById(venueId, cancellationToken);
        if (isFailure)
        {
            transactionScope.Rollback();
            return error;
        }

        venue.UpdateName(nameRequest.Name);


        await _transactionManager.SaveChangesAsync(cancellationToken);
        
        transactionScope.Commit();
        
        return nameRequest.VenueId;
    }
}