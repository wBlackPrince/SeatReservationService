using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Venue;

public record VenueId(Guid Value);

public class Venue
{
    private List<Seat> _seats = [];
    private List<Guid> _events;

    // Ef core
    private Venue()
    {
        
    }
    
    public Venue(VenueId id, VenueName name,  int maxSeatsCount, IEnumerable<Seat> seats)
    {
        Id = id;
        Name = name;
        MaxSeatsCount = maxSeatsCount;
        _seats = seats.ToList();
    }
    
    public VenueId Id { get;}
    
    public VenueName Name { get; set; }
    
    public int MaxSeatsCount {get; private set;}
    
    public IReadOnlyCollection<Seat> Seats => _seats;
    public IReadOnlyList<Guid> Events => _events;
    
    public void AddEvent(Guid @event) => _events.Add(@event);

    public void AddSeats(IEnumerable<Seat> seats) => _seats.AddRange(seats);
    
    public int SeatsCount => _seats.Count;
    
    public UnitResult<Error> AddSeat(Seat seat)
    {
        if (Seats.Count >= MaxSeatsCount)
        {
            return Error.Conflict("venue.seats.limit", "");
        }
        
        _seats.Add(seat);
        
        return UnitResult.Success<Error>();
    }

    public static Result<Venue, Error> Create(
        string prefix,
        string name,
        int seatsLimit)
    {
        if (seatsLimit <= 0)
        {
            return Error.Validation(
                "venues.seatsLimit",
                "Лимит на количество мест должен быть больше нуля !");
        }

        var venueNameResult = VenueName.Create(prefix, name);

        if (venueNameResult.IsFailure)
        {
            return venueNameResult.Error;
        }

        // var venuesSeats = seats.ToList();
        //
        // if (venuesSeats.Count < 1)
        // {
        //     return Error.Validation(
        //         "venues.seats", 
        //         "Слишком мало мест для площадки");
        // }
        //
        // if (venuesSeats.Count >= seatsLimit)
        // {
        //     return Error.Conflict(
        //         "venues.seats.limit", 
        //         "Слишком большое количество сидений");
        // }
        
        return new Venue(
            new VenueId(Guid.NewGuid()), 
            venueNameResult.Value, 
            seatsLimit,
            []);

    }
    
    public void ExpandSeatsLimit(int newSeatsLimit) => MaxSeatsCount = newSeatsLimit;
}