using Microsoft.EntityFrameworkCore;
using SeatReservationDomain.Event;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract.Events;

namespace SeatReservationService.Application.Events.Commands;

public class GetEventByIdHandler
{
    private readonly IReadDbContext _readDbContext;
    
    public GetEventByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }


    public async Task<GetEventDto?> Handle(
        GetEventByIdRequest query,
        CancellationToken cancellationToken)
    {
        var @event = await _readDbContext.EventsRead
            .Include(e => e.Details)
            .FirstOrDefaultAsync(e => e.Id == new EventId(query.EventId), cancellationToken);

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