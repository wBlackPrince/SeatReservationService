using Microsoft.AspNetCore.Mvc;
using SeatReservationService.Application.Reservations;
using SeatReservationService.Contract;
using SeatReservationService.Contract.Reservations;

namespace SeatReservationService.Controllers;


[ApiController]
[Route("api/reservations")]
public class ReservationController: ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Reserve(
        [FromBody]ReserveRequest request,
        [FromServices]ReserveHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        
        return Ok();
    }

    [HttpDelete("{reservationId:guid}")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid reservationId,
        [FromServices]DeleteReservationHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(reservationId, cancellationToken);

        return Ok();
    }
}