using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Users;

public record Fio
{
    public const int MinLength = 1;
    public const int MaxLength = 30;
    public string Value { get; private set; } = string.Empty;
    
    private Fio(string value) => Value = value;

    public static Result<Fio, Error> Create(string fio)
    {
        UnitResult<Error> result = Validate(fio);

        if (result.IsFailure)
            return result.Error;

        return new Fio(fio);
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
                "user.fio",
                "user's fio cannot be null or empty");
        }

        if (name.Length < MinLength)
        {
            return Error.Validation(
                "user.fio",
                "user's fio must be longer than " + MinLength);
        }
        
        if (name.Length > MaxLength)
        {
            return Error.Validation(
                "user.fio",
                "user's fio must be less than " + MaxLength);
        }

        return UnitResult.Success<Error>();
    }
}