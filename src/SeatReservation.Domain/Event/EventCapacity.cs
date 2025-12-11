using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Event;

public record EventCapacity
{
    public const int MinCapacity = 1;
    public const int MaxCapacity = 100000;
    public int Value { get; private set; }
    
    private EventCapacity(int value) => Value = value;

    public static Result<EventCapacity, Error> Create(int capacity)
    {
        UnitResult<Error> result = Validate(capacity);

        if (result.IsFailure)
            return result.Error;

        return new EventCapacity(capacity);
    }
    
    public UnitResult<Error> Update(int newCapacity)
    {
        UnitResult<Error> result = Validate(newCapacity);

        Value = newCapacity;

        return UnitResult.Success<Error>();
    }

    public static UnitResult<Error> Validate(int capacity)
    {
        if (capacity < MinCapacity)
        {
            return Error.Validation(
                "event.capacity",
                "event capacity must be longer than " + MinCapacity);
        }
        
        if (capacity > MaxCapacity)
        {
            return Error.Validation(
                "event.capacity",
                "event capacity must be less than " + MaxCapacity);
        }

        return UnitResult.Success<Error>();
    }
}