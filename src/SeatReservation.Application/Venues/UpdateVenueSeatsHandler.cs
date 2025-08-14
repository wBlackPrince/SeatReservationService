using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract;

namespace SeatReservationService.Application.Venues;

public class UpdateVenueSeatsHandler
{
    public readonly IVenuesRepository _venuesRepository;

    public UpdateVenueSeatsHandler(IVenuesRepository venuesRepository)
    {
        _venuesRepository = venuesRepository;
    }
    
    
    public async Task<Result<Guid, Error>> Handle(
        UpdateVenueSeatsRequest request,
        CancellationToken cancellationToken)
    {
        var venueId = new VenueId(request.VenueId);
        
        await _venuesRepository.DeleteSeatsByVenueId(venueId, cancellationToken);
        
        var venue = await _venuesRepository.GetById(venueId, cancellationToken);
        if (venue.IsFailure)
        {
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
        

        await _venuesRepository.Save();
        return request.VenueId;
    }
}