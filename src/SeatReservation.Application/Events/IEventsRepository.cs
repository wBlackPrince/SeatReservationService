using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Event;

namespace SeatReservationService.Application.Events;

public interface IEventsRepository
{
    
    Task<Result<Event, Error>> GetById(EventId eventId, CancellationToken cancellationToken);

    Task<Result<Event, Error>> GetAvailableForReservation(EventId eventId, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> Create(Event @event, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}