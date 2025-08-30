using SeatReservationDomain.Event;
using SeatReservationService.Contract.Events;

namespace SeatReservationService.Application.Events;

public class GetEventByIdHandler
{
    private readonly IEventsRepository _eventsRepository;
    
    public GetEventByIdHandler(IEventsRepository eventsRepository)
    {
        _eventsRepository = eventsRepository;
    }


    public async Task<GetEventDto?> Handle(
        GetEventByIdRequest request,
        CancellationToken cancellationToken)
    {
        var @event = await _eventsRepository.GetById(new EventId(request.EventId), cancellationToken);

        if (@event is null)
        {
            return null;
        }

        return new GetEventDto()
        {
            Id = @event.Id.Value,
            Capacity = @event.Details.Capacity,
            Description = @event.Details.Description,
            LastReservationUtc = @event.Details.LastReservationUtc,
            VenueId = @event.VenueId.Value,
            EventDate = @event.EventDate,
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            Status = @event.Status.ToString(),
            Type = @event.Type.ToString(),
            Info = @event.Info.ToString()
        };
    }
}