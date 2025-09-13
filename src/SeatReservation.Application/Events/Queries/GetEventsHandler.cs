using System.Data;
using Dapper;
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



public class GetEventsHandlerDapper
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetEventsHandlerDapper(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetEventsDto> Handle(
        GetEventsRequest query, 
        CancellationToken cancellationToken)
    {
        var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        
        List<string> conditions = new List<string>();
        
        
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            parameters.Add("search", query.Search, DbType.String);
            conditions.Add("e.name ILIKE '%' || @search || '%'");
        }
        
        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            parameters.Add("type", query.EventType, DbType.String);
            conditions.Add("e.type = @type");
        }
        
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            parameters.Add("status", query.Status, DbType.String);
            conditions.Add("e.status = @status");
        }
        
        if (query.DateFrom is not null)
        {
            parameters.Add("start_date", query.DateFrom?.ToUniversalTime(), DbType.DateTime);
            conditions.Add("e.start_date >= @start_date");
        }
        
        if (query.DateFrom is not null)
        {
            parameters.Add("end_date", query.DateTo?.ToUniversalTime(), DbType.DateTime);
            conditions.Add("e.end_date <= @end_date");
        }
        
        if (query.VenueId is not null)
        {
            parameters.Add("venue_id", query.VenueId, DbType.Guid);
            conditions.Add("e.venue_id = @venue_id");
        }
        
        if (query.MinAvailableSeats.HasValue)
        {
            conditions.Add("""
                           ((SELECT COUNT(*) FROM seats s WHERE s.venue_id = e.venue_id) - 
                            COALESCE((SELECT COUNT(*)
                                      FROM reservation_seats rs
                                               JOIN reservations r ON rs.reservation_id = r.id
                                      WHERE rs.event_id = e.id
                                        AND r.status IN ('Confirmed', 'Pending')), 0)) >= @min_available_seats
                           """);
            parameters.Add("min_available_seats", query.MinAvailableSeats.Value);
        }
        
        parameters.Add("offset", (query.PaginationRequest.Page - 1) * query.PaginationRequest.PageSize, DbType.Int32);
        parameters.Add("limit", query.PaginationRequest.PageSize, DbType.Int32);

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

        long? totalCount = null;
        
        var events = await connection.QueryAsync<EventDto, long, EventDto>(
            $"""
            SELECT
                e.id,
                e.venue_id,
                e.name,
                e.event_date,
                e.start_date,
                e.end_date,
                e.status,
                e.type,
                e.info,
                ed.capacity,
                ed.description,
                
                (select count(*) from seats as s 
                    where s.venue_id = e.venue_id ) AS total_seats,
                
                (select count(*) from reservation_seats as rs JOIN reservations as r ON rs.reservation_id = r.id
                    where rs.event_id = e.id 
                      and r.status in ('Pending', 'Confirmed')) AS reserved_seats,
            
                (select count(*) from seats as s 
                    where s.venue_id = e.venue_id ) - (select count(*) from reservation_seats as rs JOIN reservations as r ON rs.reservation_id = r.id
                    where rs.event_id = e.id 
                      and r.status in ('Pending', 'Confirmed')) AS available_seats,
                count(*) over () as total_count
            FROM events AS e JOIN event_details as ed ON e.id = ed.event_id
            {whereClause}
            LIMIT @limit
            OFFSET @offset
            """,
            splitOn: "total_count",
            map: (ed, tc) =>
            {
                totalCount ??= tc;

                return ed;
            },
            param: parameters);
        
        return new GetEventsDto(events.ToList(), totalCount ?? 0);
    }
}