using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Application.Venues;

public interface IVenuesRepository
{
    Task<Result<Guid, Error>> Add(
        Venue venue, 
        CancellationToken cancellationToken = default);
    
    Task<Result<Guid, Error>> UpdateVenueName(
        Guid venueId, 
        VenueName venueName, 
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> UpdateVenueNameByPrefix(
        string prefix,
        VenueName venueName,
        CancellationToken cancellationToken);

    Task<Result<Venue, Error>> GetById(
        Guid id,
        CancellationToken cancellationToken);

    Task Update(Venue venue, CancellationToken cancellationToken);

    Task<IReadOnlyList<Venue>> GetByPrefix(
        string prefix,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteSeatsByVenueId(
        Guid venueId,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> AddSeats(
        IEnumerable<Seat> seats,
        CancellationToken cancellationToken);

    Task<Result<Venue, Error>> GetByIdWithSeats(
        Guid id,
        CancellationToken cancellationToken);
}