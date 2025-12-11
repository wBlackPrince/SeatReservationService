using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Event;

public record EventDescription
{
    public const int MinLength = 2;
    public const int MaxLength = 3000;
    public string Value { get; private set; } = string.Empty;
    
    private EventDescription(string value) => Value = value;

    public static Result<EventDescription, Error> Create(string description)
    {
        UnitResult<Error> result = Validate(description);

        if (result.IsFailure)
            return result.Error;

        return new EventDescription(description);
    }
    
    public UnitResult<Error> Update(string newDescription)
    {
        UnitResult<Error> result = Validate(newDescription);

        Value = newDescription;

        return UnitResult.Success<Error>();
    }

    public static UnitResult<Error> Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation(
                "event.description",
                "event description cannot be null or empty");
        }

        if (name.Length < MinLength)
        {
            return Error.Validation(
                "event.description",
                "event description must be longer than " + MinLength);
        }
        
        if (name.Length > MaxLength)
        {
            return Error.Validation(
                "event.description",
                "event description must be less than " + MaxLength);
        }

        return UnitResult.Success<Error>();
    }
}