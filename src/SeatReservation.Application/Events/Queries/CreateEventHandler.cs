using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;
using SeatReservationService.Contract.Events;

namespace SeatReservationService.Application.Events.Queries;

public class CreateConcertHandler
{
    private readonly IEventsRepository _eventsRepository;
    private readonly ILogger<CreateConcertHandler> _logger;

    public CreateConcertHandler(
        IEventsRepository eventsRepository,
        ILogger<CreateConcertHandler> logger)
    {
        _eventsRepository = eventsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Handle(CreateConcertRequest request, CancellationToken cancellationToken)
    {
        var concert = Event.CreateConcert(
            new VenueId(request.VenueId),
            request.Name,
            request.EventDate,
            request.StartDate,
            request.EndDate,
            request.Capacity,
            request.Description,
            request.Performer);

        if (concert.IsFailure)
        {
            _logger.LogError($"Concert {concert.Error.Message}");
            return concert.Error;
        }
        
        await _eventsRepository.Create(concert.Value, cancellationToken);
        
        await _eventsRepository.SaveChangesAsync(cancellationToken);
        
        return concert.Value.Id.Value;
    }
}