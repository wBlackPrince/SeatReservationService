using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Contract.venues;

namespace SeatReservationService.Application.Venues.Commands;

public class UpdateVenueHandler
{
    public readonly IVenuesRepository _venuesRepository;

    public UpdateVenueHandler(IVenuesRepository venuesRepository)
    {
        _venuesRepository = venuesRepository;
    }
    
    
    public async Task<Result<Guid, Error>> Handle(
        UpdateVenueRequest request,
        CancellationToken cancellationToken)
    {
        var venueId = request.VenueId;
        
        var venue = Venue.Create(
            request.Prefix,
            request.Name,
            request.MaxSeatsLimit,
            venueId);

        if (venue.IsFailure)
        {
            return venue.Error;
        }
        


        await _venuesRepository.Update(venue.Value, cancellationToken);
        
        return request.VenueId;
    }
}