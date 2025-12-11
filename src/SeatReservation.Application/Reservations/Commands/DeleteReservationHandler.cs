using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain.Reservation;

namespace SeatReservationService.Application.Reservations.Commands;

public class DeleteReservationHandler
{
    private readonly IReservationsRepository _reservationsRepository;
    private readonly ILogger<DeleteReservationHandler> _logger;
    
    public DeleteReservationHandler(
        IReservationsRepository venuesRepository,
        ILogger<DeleteReservationHandler> logger)
    {
        _reservationsRepository = venuesRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Handle(Guid reservationId, CancellationToken cancellationToken)
    {
        var deleteResult = await _reservationsRepository.Delete(
            reservationId, 
            cancellationToken);

        if (deleteResult.IsFailure)
        {
            _logger.LogInformation("Reservation {reservationId} was not deleted.", reservationId);
            return deleteResult.Error;
        }
        
        await _reservationsRepository.SaveChangesAsync(cancellationToken);
        
        return reservationId;
    }
}