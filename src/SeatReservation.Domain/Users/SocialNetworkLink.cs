using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Users;

public record SocialNetworkLink
{
    public const int MinLength = 5;
    public const int MaxLength = 1000;
    public string Value { get; private set; } = string.Empty;
    
    private SocialNetworkLink(string value) => Value = value;

    public static Result<SocialNetworkLink, Error> Create(string description)
    {
        UnitResult<Error> result = Validate(description);

        if (result.IsFailure)
            return result.Error;

        return new SocialNetworkLink(description);
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
                "user.social_network_link",
                "user's social_network link cannot be null or empty");
        }

        if (description.Length < MinLength)
        {
            return Error.Validation(
                "user.social_network_link",
                "user's social_network link must be longer than " + MinLength);
        }
        
        if (description.Length > MaxLength)
        {
            return Error.Validation(
                "user.social_network_link",
                "user's social_network link must be less than " + MaxLength);
        }

        return UnitResult.Success<Error>();
    }
}