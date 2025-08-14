using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract;

namespace SeatReservationService.Application.Venues;

public class UpdateVenueNameHandler
{
    public readonly IVenuesRepository _venuesRepository;

    public UpdateVenueNameHandler(IVenuesRepository venuesRepository)
    {
        _venuesRepository = venuesRepository;
    }
    
    
    public async Task<Result<Guid, Error>> Handle(
        UpdateVenueNameRequest nameRequest,
        CancellationToken cancellationToken)
    {
        var venueId = new VenueId(nameRequest.VenueId);
        

        var (_, isFailure, venue, error) = await _venuesRepository.GetById(venueId, cancellationToken);
        if (isFailure)
        {
            return error;
        }

        venue.UpdateName(nameRequest.Name);


        await _venuesRepository.Save();
        
        return nameRequest.VenueId;
    }
}