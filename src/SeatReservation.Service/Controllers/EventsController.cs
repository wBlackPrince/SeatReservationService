using Microsoft.AspNetCore.Mvc;
using SeatReservation.Infrastructure.Postgres.Repositories;
using SeatReservationService.Application.Events;
using SeatReservationService.Application.Events.Commands;
using SeatReservationService.Application.Events.Queries;
using SeatReservationService.Contract;
using SeatReservationService.Contract.Events;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController: ControllerBase
{
    [HttpGet("/{eventId:guid}")]
    public async Task<ActionResult<GetEventDto>> GetById(
        [FromServices] GetEventByIdHandler handler,
        [FromRoute]Guid eventId, 
        CancellationToken cancellationToken)
    {
        var @event = await handler.Handle(new GetEventByIdRequest(eventId), cancellationToken);
        return Ok(@event);
    }
    
    [HttpGet("/{eventId:guid}/dapper")]
    public async Task<ActionResult<GetEventDto>> GetByIdDapper(
        [FromServices] GetEventByIdHandlerDapper handler,
        [FromRoute]Guid eventId, 
        CancellationToken cancellationToken)
    {
        var @event = await handler.Handle(new GetEventByIdRequest(eventId), cancellationToken);
        return Ok(@event);
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> Reserve(
        [FromBody] CreateConcertRequest request,
        [FromServices]CreateConcertHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        
        return Ok(result.Value);
    }
}