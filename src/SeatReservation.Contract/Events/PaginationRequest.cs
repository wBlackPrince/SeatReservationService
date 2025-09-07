namespace SeatReservationService.Contract.Events;

public record PaginationRequest(
    int Page = 1,
    int PageSize = 20);