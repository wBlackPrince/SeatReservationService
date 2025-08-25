using System.Data;
using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Event;
using SeatReservationDomain.Reservation;
using SeatReservationDomain.Venue;
using SeatReservationService.Application.Database;
using SeatReservationService.Application.Events;
using SeatReservationService.Application.Seats;
using SeatReservationService.Contract.Reservations;

namespace SeatReservationService.Application.Reservations;

public class ReserveAdjacentSeatsHandler
{
    public readonly ISeatsRepository _seatsRepository;
    public readonly IReservationsRepository _reservationsRepository;
    public readonly IEventsRepository _eventsRepository;
    public ITransactionManager _transactionManager;

    public ReserveAdjacentSeatsHandler(
        ISeatsRepository seatsRepository,
        IReservationsRepository reservationsRepository,
        IEventsRepository eventsRepository,
        ITransactionManager transactionManager)
    {
        _seatsRepository = seatsRepository;
        _reservationsRepository = reservationsRepository;
        _eventsRepository = eventsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Error>> Handle(
        ReserveAdjacentSeatsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.RequiredSeatsCount <= 0)
        {
            return Error.Failure(
                "reserve.adjacent_seats",
                "Seat's count cannot be below zero");
        }

        if (request.RequiredSeatsCount > 10)
        {
            return Error.Failure(
                "reserve.adjacent_seats",
                "Seat's count cannot be upper 10");
        }
        
        var transactionResult = await _transactionManager.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        if (transactionResult.IsFailure)
        {
            return transactionResult.Error;
        }
        
        using var transaction = transactionResult.Value;
        
        EventId eventId = new EventId(request.EventId);
        VenueId venueId = new VenueId(request.VenueId);
        
        var (_, isFailure, @event, error) = await _eventsRepository.GetByIdWithLock(
            eventId, 
            cancellationToken);

        if (isFailure)
        {
            transaction.Rollback();
            return error;
        }

        var availableSeats = await _seatsRepository.GetAvailableSeats(
            venueId,
            eventId,
            request.PreferredRowNumber,
            cancellationToken
        );

        if (availableSeats.Count == 0)
        {
            transaction.Rollback();
            return Error.Failure("reserveAdjacentSeats.seats", "No available seats found");
        }
        
        var selectedSeats = request.PreferredRowNumber.HasValue 
            ? AdjacentSeatsFinder.FindAdjacentSeatsInPreferredRow(
                availableSeats, 
                request.RequiredSeatsCount, 
                request.PreferredRowNumber.Value) 
            : AdjacentSeatsFinder.FindBestAdjacentSeats(availableSeats, request.RequiredSeatsCount);

        if (selectedSeats.Count == 0)
        {
            transaction.Rollback();
            return Error.Failure("reserveAdjacentSeats.seats", "No selected seats found");
        }

        if (selectedSeats.Count < request.RequiredSeatsCount)
        {
            transaction.Rollback();
            return Error.Failure(
                "reserveAdjacentSeats.seats", 
                $"Only {selectedSeats.Count} seats are available for reservation");
        }

        var seatsIds = selectedSeats.Select(s => s.Id).ToList();

        var reservation = Reservation.Create(
            eventId.Value,
            request.UserId,
            seatsIds.Select(si => si.Value).ToList());

        var addResult = await _reservationsRepository.Add(reservation, cancellationToken);

        if (addResult.IsFailure)
        {
            transaction.Rollback();
            return addResult.Error;
        }
        
        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            transaction.Rollback();
            return saveResult.Error;
        }
        
        var commitResult = transaction.Commit();
        if (commitResult.IsFailure)
        {
            return commitResult.Error;
        }

        return Result.Success<Guid, Error>(addResult.Value);
    }
}