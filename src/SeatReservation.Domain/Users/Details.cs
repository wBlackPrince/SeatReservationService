using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Users;

public record Details
{
    private List<SocialNetwork> _socials;

    private Details()
    {
    }

    private Details(UserDescription userDescription, Fio fio)
    {
        Description = userDescription;
        FIO = fio;
    }

    public static Result<Details, Error> Create(string description, string fio)
    {
        Result<UserDescription, Error> userDescriptionResult = UserDescription.Create(description);

        if (userDescriptionResult.IsFailure)
            return userDescriptionResult.Error;
        
        Result<Fio, Error> fioResult = Fio.Create(fio);

        if (fioResult.IsFailure)
            return fioResult.Error;
        
        return new Details(userDescriptionResult.Value, fioResult.Value);
    }

    public UserDescription Description { get; set; }
    public Fio FIO { get; set; }

    public IReadOnlyList<SocialNetwork> Socials
    {
        get { return _socials; }
        set { _socials = value.ToList(); }
    }
}