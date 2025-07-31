using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Venue;

public record VenueName
{
    private VenueName(string prefix, string name)
    {
        Prefix = prefix;
        Name = name;
    }
    
    public string Prefix {get; }
    public string Name {get; }

    public override string ToString() => $"{Prefix}-{Name}";

    public static Result<VenueName, Error> Create(string prefix, string name)
    {
        if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("venue.name", "Имя площадки не должно быть пустым");
        }

        if ((prefix.Length > LengthConstants.Length50)
            || (name.Length > LengthConstants.Length500))
        {
            return Error.Validation("venue.name", "Имя площадки слишком длинное");
        }

        return new VenueName(prefix, name);
    }
}