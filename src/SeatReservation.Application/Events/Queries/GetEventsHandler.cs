using Microsoft.EntityFrameworkCore;
using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract.Events;

namespace SeatReservationService.Application.Events.Queries;

public class GetEventsHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetEventsHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetEventsDto> Handle(GetEventsRequest query, CancellationToken cancellationToken)
    {
        var eventsQuery = _readDbContext.EventsRead;
        
        if(!string.IsNullOrWhiteSpace(query.Search))
            eventsQuery = eventsQuery
                .Where(e => EF.Functions.Like(
                    e.Name.ToLower(),  
                    $"%{query.Search.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            EventType enumType = (EventType)Enum.Parse(typeof(EventType), query.EventType);
            
            eventsQuery = eventsQuery.Where(e => e.Type == enumType);
        }
        
        if (query.DateFrom is not null)
            eventsQuery = eventsQuery.Where(e => e.EventDate >= query.DateFrom.Value.ToUniversalTime());
        
        if (query.DateTo is not null)
            eventsQuery = eventsQuery.Where(e => e.EventDate <= query.DateTo.Value.ToUniversalTime());
        
        if (query.VenueId.HasValue)
            eventsQuery = eventsQuery.Where(e => e.VenueId == new VenueId(query.VenueId.Value));

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            EventStatus enumStatus = (EventStatus)Enum.Parse(typeof(EventStatus), query.Status);
            
            eventsQuery = eventsQuery.Where(e => e.Status == enumStatus);
        }

        if (query.MinAvailableSeats.HasValue)
        {
            eventsQuery = eventsQuery.Where(e =>
                _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) -
                _readDbContext.ReservationSeatsRead.Count(rs =>
                    rs.EventId == e.Id &&
                    (rs.Reservation.Status == ReservationStatus.Confirmed ||
                     rs.Reservation.Status == ReservationStatus.Pending))
                >= query.MinAvailableSeats.Value);
        }
        
        var totalCount = await eventsQuery.LongCountAsync();
        
        eventsQuery = eventsQuery
            .OrderBy(e => e.EventDate)
            .Skip((query.PaginationRequest.Page - 1) * query.PaginationRequest.PageSize)
            .Take(query.PaginationRequest.PageSize);
        
        var events = await eventsQuery
            .Select(e => new EventDto(){
                Id = e.Id.Value,
                Capacity = e.Details.Capacity,
                Description = e.Details.Description,
                LastReservationUtc = e.Details.LastReservationUtc,
                VenueId = e.VenueId.Value,
                Name = e.Name,
                EventDate = e.EventDate,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Status = e.Status.ToString(),
                Type = e.Type.ToString(),
                Info = e.Info.ToString(),
                TotalSeats = _readDbContext.SeatsRead
                .Count(s => s.VenueId == e.VenueId),
                ReservedSeats = _readDbContext.ReservationSeatsRead
                    .Count(rs => rs.EventId == e.Id),
                AvailableSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) - 
                                 _readDbContext.ReservationSeatsRead.Count(
                                     rs => rs.EventId == e.Id && (rs.Reservation.Status == ReservationStatus.Confirmed ||
                                                                  rs.Reservation.Status == ReservationStatus.Pending)),
            })
            .ToListAsync(cancellationToken);

        return new GetEventsDto(events, totalCount);
    }
}