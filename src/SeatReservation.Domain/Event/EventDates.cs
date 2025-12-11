using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Event;

public record EventDates
{
    public DateTime EventDate { get; private set; }

    public DateTime StartDate { get; private set; }
    
    public DateTime EndDate { get; private set; }

    private EventDates(DateTime eventDate, DateTime startDate, DateTime endDate)
    {
        EventDate = eventDate;
        StartDate = startDate;
        EndDate = endDate;
    }
    
    
    public static Result<EventDates, Error> Create(
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate)
    {
        UnitResult<Error> result = Validate(eventDate, startDate, endDate);

        if (result.IsFailure)
            return result.Error;

        return new EventDates(eventDate, startDate, endDate);
    }
    
    public UnitResult<Error> UpdateEventDate(DateTime eventDate)
    {
        UnitResult<Error> result = ValidateEventDate(eventDate);

        EventDate = eventDate;

        return UnitResult.Success<Error>();
    }
    
    public UnitResult<Error> UpdateEventStartEndDate(DateTime startDate, DateTime endDate)
    {
        UnitResult<Error> result = ValidateEventStartEndDate(startDate, endDate);

        StartDate = startDate;
        EndDate = endDate;

        return UnitResult.Success<Error>();
    }


    public static UnitResult<Error> Validate(
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate)
    {
        UnitResult<Error> validateEventDateResult = ValidateEventDate(eventDate);

        if (validateEventDateResult.IsFailure)
            return UnitResult.Failure<Error>(validateEventDateResult.Error);

        UnitResult<Error> validateEventStartEndResult = ValidateEventStartEndDate(startDate, endDate);

        if (validateEventDateResult.IsFailure)
            return UnitResult.Failure<Error>(validateEventDateResult.Error);
        

        return UnitResult.Success<Error>();
    }
    
    public static UnitResult<Error> ValidateEventDate(
        DateTime eventDate)
    {
        if (eventDate < DateTime.UtcNow)
        {
            return Error.Validation("event.date", "Event date cannot be in the past");
        }

        return UnitResult.Success<Error>();
    }
    
    public static UnitResult<Error> ValidateEventStartEndDate(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate >= endDate || startDate <= DateTime.UtcNow || endDate < DateTime.UtcNow)
        {
            return Error.Validation("event.date", "Time of event is incorrect");
        }

        return UnitResult.Success<Error>();
    }
}