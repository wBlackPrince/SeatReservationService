using Microsoft.AspNetCore.Mvc;
using SeatReservation.Infrastructure.Postgres.Repositories;
using SeatReservationService.Application.Events;
using SeatReservationService.Contract;
using SeatReservationService.Contract.Events;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController: ControllerBase
{
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