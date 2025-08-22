using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SeatReservation.Infrastructure.Postgres.Repositories;
using SeatReservation.Shared;
using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Contract;
using EventId = SeatReservationDomain.Event.EventId;

namespace SeatReservationService.Application.Reservations;

public class ReserveHandler
{
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly ISeatsRepository _seatsRepository;
    private readonly ILogger<ReserveHandler> _logger;
    private readonly IValidator<ReserveRequest> _reverveValidator;
    
    public ReserveHandler(
        IReservationsRepository venuesRepository,
        IEventsRepository eventsRepository,
        ISeatsRepository seatsRepository,
        ILogger<ReserveHandler> logger,
        IValidator<ReserveRequest> reverveValidator)
    {
        _reservationsRepository = venuesRepository;
        _eventsRepository = eventsRepository;
        _seatsRepository = seatsRepository;
        _logger = logger;
        _reverveValidator = reverveValidator;
    }

    /// <summary>
    /// Бронирует места на площадке
    /// </summary>
    public async Task<Result<Guid, Error>> Handle(
        ReserveRequest request,
        CancellationToken cancellationToken)
    {
        // бронирование мест на мероприятие
        
        // 1. валидация входных параметров
        
        var validationResult = await _reverveValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Error.Failure("reservation.validation_request", "reservation is invalid");
        }
        
        // 2. Доступно ли бронирование для мероприятия. Проверить даты. Проверить статус.

        var eventId = new EventId(request.EventId);
        var (_, isFailure, @event, error) = await _eventsRepository.GetById(eventId, cancellationToken);

        if (isFailure)
        {
            return error;
        }
        
        if (!@event.IsAvailableForReservation())
        {
            return Error.Failure("reservation", "reservation is unavailable");
        }
        
        // 3. Проверить что места принадлежат нужной площадке и мероприятию
        var seatsIds = request.Seats.Select(i => new SeatId(i)).ToList();
        var seats = _seatsRepository.GetByIds(seatsIds, cancellationToken).Result;


        if(seats.Any(seat => (seat.VenueId != @event.VenueId)) && seats.Count == 0)
        {
            return Error.Conflict("reservation.conflict", "Seat is not from this venue");
        }
        
        
        // 4. Проверить что места не забронированы
        bool isAnySeatAlreadyReserved = await _reservationsRepository.AnySeatsAlreadyReserved(
            request.EventId, 
            seatsIds, 
            cancellationToken);

        if (isAnySeatAlreadyReserved)
        {
            return Error.Conflict("reservation.conflict", "Seat is already reserved");
        }
        
        // создать Reservation с reserved seats
        var reservation = Reservation.Create(request.EventId, request.UserId, request.Seats);


        
        // сохранение бронирования в бд
        var reservationId = await _reservationsRepository.Add(reservation, cancellationToken);

        if (reservationId.IsFailure)
        {
            return Error.Failure("reservation", "reservation.failure");
        }
        
        return reservationId.Value;
    }
}