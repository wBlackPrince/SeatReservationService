namespace SeatReservationService.Contract.Seats;

public record UpdateSeatRequest(int rowNumber, int seatNumber);