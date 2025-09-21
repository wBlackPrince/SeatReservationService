namespace SeatReservationService.Contract.Events;

public record GetEventsRequest(        
    string? Search,
    string? EventType,
    DateTime? DateFrom,
    DateTime? DateTo,
    string? Status,
    Guid? VenueId,
    int? MinAvailableSeats,
    PaginationRequest PaginationRequest,
    string? SortBy = "sort",
    string? SortDirection = "desc");