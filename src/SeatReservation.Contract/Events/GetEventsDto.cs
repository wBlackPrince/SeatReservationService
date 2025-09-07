namespace SeatReservationService.Contract.Events;

public record GetEventsDto(List<EventDto> Events, long TotalCount);