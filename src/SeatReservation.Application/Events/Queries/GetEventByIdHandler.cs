using Dapper;
using Microsoft.EntityFrameworkCore;
using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract.Events;

namespace SeatReservationService.Application.Events.Queries;

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
        var eventDto = await _readDbContext.EventsRead
            .Include(e => e.Details)
            .Where(e => e.Id == new EventId(query.EventId))
            .Select(e => new GetEventDto()
            {
                Id = e.Id.Value,
                Capacity = e.Details.Capacity,
                Description = e.Details.Description,
                LastReservationUtc = e.Details.LastReservationUtc,
                VenueId = e.VenueId.Value,
                EventDate = e.EventDate,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Status = e.Status.ToString(),
                Type = e.Type.ToString(),
                Info = e.Info.ToString(),
                Seats = (from s in _readDbContext.SeatsRead
                        where s.VenueId == e.VenueId
                        //join ev in _readDbContext.EventsRead on s.VenueId equals ev.VenueId
                        join rs in _readDbContext.ReservationSeatsRead on 
                            new {SeatId = s.Id, EventId = e.Id} equals 
                            new {SeatId = rs.SeatId, EventId = rs.EventId} into reservations
                        from r in reservations.DefaultIfEmpty()
                        where e.Id == new EventId(query.EventId)
                        orderby s.RowNumber, s.SeatNumber
                        select new ReservedSeatsDto
                        {
                            Id = s.Id.Value,
                            RowNumber = s.RowNumber,
                            SeatNumber = s.SeatNumber,
                            VenueId = s.VenueId.Value,
                            IsAvailable = r == null
                        }).ToList(),
                TotalSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId),
                ReservedSeats = _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == e.Id),
                AvailableSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) -
                                 _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == e.Id &&
                                     (rs.Reservation.Status == ReservationStatus.Confirmed ||
                                      rs.Reservation.Status == ReservationStatus.Pending))
                
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (eventDto is null)
        {
            return null;
        }
        
        return eventDto;
    }
}

public class GetEventByIdHandlerDapper
{
    private readonly IDbConnectionFactory _connectionFactory;
    
    public GetEventByIdHandlerDapper(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    public async Task<GetEventDto?> Handle(
        GetEventByIdRequest query,
        CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        GetEventDto? eventDto = null;
        
        await connection.QueryAsync<GetEventDto, ReservedSeatsDto, GetEventDto>(
            """
            SELECT 
                e.id,
                e.venue_id,
                e.name,
                e.type,
                e.event_date,
                e.start_date,
                e.end_date,
                e.status,
                e.info,
                ed.capacity,
                ed.description,
                COUNT(*) over() as total_seats,
                COUNT(rs.seat_id) over() as reserved_seats,
                COUNT(*) over() - COUNT(rs.seat_id) over() as available_seats,
                s.id,
                s.venue_id,
                s.row_number,
                s.seat_number,
                rs."Id" is null as is_available
            FROM events as e 
                INNER JOIN event_details as ed ON e.id = ed.event_id
                INNER JOIN seats as s ON s.venue_id = e.venue_id
                LEFT JOIN reservation_seats as rs ON s.id = rs.seat_id and rs.event_id = e.id
            WHERE e.id = @eventId
            ORDER BY s.row_number, s.seat_number                               
            """,
            param: new
            {
                @eventId = query.EventId
            },
            splitOn: "id",
            map: (e, s) =>
            {
                eventDto ??= e;
                eventDto.Seats.Add(s);

                return eventDto;
            });
        
        return eventDto;
    }
}