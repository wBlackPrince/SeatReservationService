using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Users;

public record UserDescription
{
    public const int MinLength = 0;
    public const int MaxLength = 3000;
    public string Value { get; private set; } = string.Empty;
    
    private UserDescription(string value) => Value = value;

    public static Result<UserDescription, Error> Create(string description)
    {
        UnitResult<Error> result = Validate(description);

        if (result.IsFailure)
            return result.Error;

        return new UserDescription(description);
    }
    
    public UnitResult<Error> Update(string newDescription)
    {
        UnitResult<Error> result = Validate(newDescription);

        Value = newDescription;

        return UnitResult.Success<Error>();
    }

    public static UnitResult<Error> Validate(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Error.Validation(
                "user.description",
                "user's description cannot be null or empty");
        }

        if (description.Length < MinLength)
        {
            return Error.Validation(
                "user.description",
                "user's description must be longer than " + MinLength);
        }
        
        if (description.Length > MaxLength)
        {
            return Error.Validation(
                "user.description",
                "user's description must be less than " + MaxLength);
        }

        return UnitResult.Success<Error>();
    }
}