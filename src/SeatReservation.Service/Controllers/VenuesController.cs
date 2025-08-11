using Microsoft.AspNetCore.Mvc;
using SeatReservationService.Application;
using SeatReservationService.Contract;

namespace SeatReservationService.Controllers;

[ApiController]
[Route("api/venues")]
public class VenuesController: ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices] CreateVenueHandler handler,
        [FromBody] CreateVenueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        
        return Ok(result.Value);
    }
}