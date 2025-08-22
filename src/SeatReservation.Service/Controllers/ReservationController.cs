using Microsoft.AspNetCore.Mvc;
using SeatReservationService.Application.Reservations;
using SeatReservationService.Contract;

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
        
        return Ok(result.Value);
    }
}