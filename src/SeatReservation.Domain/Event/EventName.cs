using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Event;

public record EventName
{
    public const int MinLength = 1;
    public const int MaxLength = 100;
    public string Value { get; private set; } = string.Empty;
    
    private EventName(string value) => Value = value;

    public static Result<EventName, Error> Create(string name)
    {
        UnitResult<Error> result = Validate(name);

        if (result.IsFailure)
            return result.Error;

        return new EventName(name);
    }
    
    public UnitResult<Error> Update(string newName)
    {
        UnitResult<Error> result = Validate(newName);

        Value = newName;

        return UnitResult.Success<Error>();
    }

    public static UnitResult<Error> Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation(
                "event.name",
                "event name cannot be null or empty");
        }

        if (name.Length < MinLength)
        {
            return Error.Validation(
                "event.name",
                "event name must be longer than " + MinLength);
        }
        
        if (name.Length > MaxLength)
        {
            return Error.Validation(
                "event.name",
                "event name must be less than " + MaxLength);
        }

        return UnitResult.Success<Error>();
    }
}