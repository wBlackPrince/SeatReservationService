using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Contract.venues;

namespace SeatReservationService.Application.Venues.Commands;

public class UpdateVenueNameByPrefixHandler
{
    public readonly IVenuesRepository _venuesRepository;

    public UpdateVenueNameByPrefixHandler(IVenuesRepository venuesRepository)
    {
        _venuesRepository = venuesRepository;
    }
    
    
    public async Task<UnitResult<Error>> Handle(
        UpdateVenueNameByPrefixRequest request,
        CancellationToken cancellationToken)
    {
        var venues = await _venuesRepository.GetByPrefix(
            request.Prefix, 
            cancellationToken);
        
        var venueName = VenueName.CreateWithoutPrefix(request.Name);
        if (venueName.IsFailure)
        {
            return venueName.Error;
        }
        
        var result = await _venuesRepository.UpdateVenueNameByPrefix(
            request.Prefix, 
            venueName.Value, 
            cancellationToken);
        
        if (result.IsFailure)
        {
            return result.Error;
        }

        return UnitResult.Success<Error>();
    }
}