using System.Data;
using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Application.Events;
using SeatReservationService.Application.Seats;
using SeatReservationService.Contract;
using SeatReservationService.Contract.Reservations;
using EventId = SeatReservationDomain.Event.EventId;

namespace SeatReservationService.Application.Reservations;

public class ReserveHandler
{
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly ISeatsRepository _seatsRepository;
    private readonly ILogger<ReserveHandler> _logger;
    private readonly IValidator<ReserveRequest> _reverveValidator;
    private readonly ITransactionManager _transactionManager;
    
    public ReserveHandler(
        IReservationsRepository venuesRepository,
        IEventsRepository eventsRepository,
        ISeatsRepository seatsRepository,
        ILogger<ReserveHandler> logger,
        IValidator<ReserveRequest> reverveValidator,
        ITransactionManager transactionManager)
    {
        _reservationsRepository = venuesRepository;
        _eventsRepository = eventsRepository;
        _seatsRepository = seatsRepository;
        _logger = logger;
        _reverveValidator = reverveValidator;
        _transactionManager = transactionManager;
    }

    /// <summary>
    /// Бронирует места на площадке
    /// </summary>
    public async Task<Result<Guid, Error>> Handle(
        ReserveRequest request,
        CancellationToken cancellationToken)
    {
        // бронирование мест на мероприятие
        
        
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(
            IsolationLevel.RepeatableRead,
            cancellationToken);
        if (transactionScopeResult.IsFailure)
        {
            return transactionScopeResult.Error;
        }
        
        using var transactionScope = transactionScopeResult.Value;
        
        // 1. валидация входных параметров
        
        var validationResult = await _reverveValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            transactionScope.Rollback();
            return Error.Failure("reservation.validation_request", "reservation is invalid");
        }
        
        // 2. Доступно ли бронирование для мероприятия. Проверить даты. Проверить статус.

        var eventId = new EventId(request.EventId);
        var (_, isFailure, @event, error) = await _eventsRepository.GetByIdWithLock(eventId, cancellationToken);

        if (isFailure)
        {
            transactionScope.Rollback();
            return error;
        }
        
        
        int reservedSeatsCount = await _reservationsRepository.GetReservedSeatsCount(
            request.EventId, 
            cancellationToken);

        int expectedReservedSeatsCount = reservedSeatsCount + request.Seats.Count();
        
        if (!@event.IsAvailableForReservation(expectedReservedSeatsCount))
        {
            transactionScope.Rollback();
            return Error.Failure("reservation", "reservation is unavailable");
        }
        
        
        
        // 3. Проверить что места принадлежат нужной площадке и мероприятию
        var seatsIds = request.Seats.Select(i => new SeatId(i)).ToList();
        var seats = _seatsRepository.GetByIds(seatsIds, cancellationToken).Result;


        if(seats.Any(seat => (seat.VenueId != @event.VenueId)) && seats.Count == 0)
        {
            transactionScope.Rollback();
            return Error.Conflict("reservation.conflict", "Seat is not from this venue");
        }
        
        
        // 4. Проверить что места не забронированы
        // с constraint уникальным индексом проверка не нужна
        // bool isAnySeatAlreadyReserved = await _reservationsRepository.AnySeatsAlreadyReserved(
        //     request.EventId, 
        //     seatsIds, 
        //     cancellationToken);
        //
        // if (isAnySeatAlreadyReserved)
        // {
        //     transactionScope.Rollback();
        //     return Error.Conflict("reservation.conflict", "Seat is already reserved");
        // }
        
        // создать Reservation с reserved seats
        var reservationResult = Reservation.Create(
            new EventId(request.EventId), 
            request.UserId, 
            request.Seats);

        if (reservationResult.IsFailure)
        {
            transactionScope.Rollback();
            return reservationResult.Error;
        }
        
        // сохранение бронирования в бд
        var reservationId = await _reservationsRepository.Add(
            reservationResult.Value, 
            cancellationToken);

        if (reservationId.IsFailure)
        {
            transactionScope.Rollback();
            return Error.Failure("reservation", "reservation.failure");
        }
        
        @event.Details.ReserveSeat();

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            transactionScope.Rollback();
            return saveResult.Error;
        }
        
        var commitedResult = transactionScope.Commit();

        if (commitedResult.IsFailure)
        {
            transactionScope.Rollback();
            return commitedResult.Error;
        }
        
        return reservationId.Value;
    }
}