using Microsoft.AspNetCore.Mvc;
using SeatReservationService.Application;
using SeatReservationService.Application.Venues;
using SeatReservationService.Application.Venues.Commands;
using SeatReservationService.Contract;
using SeatReservationService.Contract.venues;

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

    [HttpPatch("/name")]
    public async Task<ActionResult<Guid>> UpdateVenueName(
        [FromServices] UpdateVenueNameHandler handler,
        [FromBody] UpdateVenueNameRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        
        return Ok(result.Value);
    }
    
    [HttpPatch("/name/by-prefix")]
    public async Task<ActionResult<Guid>> UpdateVenueNameByPrefix(
        [FromServices] UpdateVenueNameByPrefixHandler handler,
        [FromBody] UpdateVenueNameByPrefixRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        
        return Ok();
    }
    
    [HttpPut]
    public async Task<ActionResult<Guid>> UpdateVenue(
        [FromServices] UpdateVenueHandler handler,
        [FromBody] UpdateVenueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        
        return Ok();
    }
    
    [HttpPatch("/venue-seats")]
    public async Task<ActionResult<Guid>> UpdateVenueSeats(
        [FromServices] UpdateVenueSeatsHandler handler,
        [FromBody] UpdateVenueSeatsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        
        return Ok();
    }
}