using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Database;

public interface IVenuesRepository
{
    Task<Result<Guid, Error>> Add(
        Venue venue, 
        CancellationToken cancellationToken = default);
    
    Task<Result<Guid, Error>> UpdateVenueName(
        VenueId venueId, 
        VenueName venueName, 
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> UpdateVenueNameByPrefix(
        string prefix,
        VenueName venueName,
        CancellationToken cancellationToken);

    Task<Result<Venue, Error>> GetById(
        VenueId id,
        CancellationToken cancellationToken);


    Task Save();

    Task Update(Venue venue, CancellationToken cancellationToken);

    Task<IReadOnlyList<Venue>> GetByPrefix(
        string prefix,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteSeatsByVenueId(
        VenueId venueId,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> AddSeats(
        IEnumerable<Seat> seats,
        CancellationToken cancellationToken);

    Task<Result<Venue, Error>> GetByIdWithSeats(
        VenueId id,
        CancellationToken cancellationToken);
}