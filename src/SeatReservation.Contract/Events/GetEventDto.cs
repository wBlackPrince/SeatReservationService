

using SeatReservationDomain.Event;
using SeatReservationDomain.Venue;

namespace SeatReservationService.Contract.Events;

public record GetEventDto
{
    public Guid Id { get; init; }
    public int Capacity { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime LastReservationUtc { get; init; }
    public Guid VenueId;
    public string Name = string.Empty;
    public DateTime EventDate;
    public DateTime StartDate;
    public DateTime EndDate;
    public string Status = string.Empty;
    public string Type = string.Empty;
    public string Info = string.Empty;
}