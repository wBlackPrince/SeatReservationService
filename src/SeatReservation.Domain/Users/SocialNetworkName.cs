using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Users;

public record SocialNetworkName
{
    public const int MinLength = 0;
    public const int MaxLength = 3000;
    public string Value { get; private set; } = string.Empty;
    
    private SocialNetworkName(string value) => Value = value;

    public static Result<SocialNetworkName, Error> Create(string name)
    {
        UnitResult<Error> result = Validate(name);

        if (result.IsFailure)
            return result.Error;

        return new SocialNetworkName(name);
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
                "user.social_network_name",
                "user's social_network_name cannot be null or empty");
        }

        if (name.Length < MinLength)
        {
            return Error.Validation(
                "user.social_network_name",
                "user's social_network_name must be longer than " + MinLength);
        }
        
        if (name.Length > MaxLength)
        {
            return Error.Validation(
                "user.social_network_name",
                "user's social_network_name must be less than " + MaxLength);
        }

        return UnitResult.Success<Error>();
    }
}