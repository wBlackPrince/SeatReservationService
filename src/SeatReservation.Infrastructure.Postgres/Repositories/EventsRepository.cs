using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain.Event;
using SeatReservationService.Application.Events;
using EventId = SeatReservationDomain.Event.EventId;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class EventsRepository : IEventsRepository
{
    ReservationServiceDbContext _dbContext;
    ILogger<EventsRepository> _logger;

    public EventsRepository(
        ReservationServiceDbContext dbContext,
        ILogger<EventsRepository> logger)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Result<Guid, Error>> Create(Event @event, CancellationToken cancellationToken){
        await _dbContext.Events.AddAsync(@event, cancellationToken);
        
        return @event.Id.Value;
    }
    
    public async Task<Result<Event, Error>> GetByIdWithLock(EventId eventId, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.Events
            .FromSql($"SELECT * from events WHERE id = {eventId.Value} FOR UPDATE")
            .Include(e => e.Details)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (@event is null)
        {
            return Error.Failure("Event.get", "Event not found");
        }
        
        return @event;
    }
    
    public async Task<Result<Event, Error>> GetAvailableForReservation(
        EventId eventId, 
        CancellationToken cancellationToken)
    {
        var @event = await _dbContext.Events
            .FirstOrDefaultAsync(
                e => (e.Id == eventId) && 
                            (e.StartDate >= DateTime.UtcNow) && 
                            (e.Status == EventStatus.Planned), 
                cancellationToken);

        if (@event is null)
        {
            return Error.Failure("Event.get", "Event is not available for reservation");
        }

        return @event;
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}