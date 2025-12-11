using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Venue;

public class Venue
{
    private List<Seat> _seats = [];
    private List<Guid> _events;

    // Ef core
    private Venue()
    {
        
    }
    
    public Venue(Guid id, VenueName name,  int maxSeatsCount, IEnumerable<Seat> seats)
    {
        Id = id;
        Name = name;
        MaxSeatsCount = maxSeatsCount;
        _seats = seats.ToList();
    }
    
    public Guid Id { get;}

    public VenueName Name { get; private set; } = null!;
    
    public int MaxSeatsCount {get; private set;}
    
    public IReadOnlyCollection<Seat> Seats => _seats;
    public IReadOnlyList<Guid> Events => _events;
    
    public void AddEvent(Guid @event) => _events.Add(@event);

    public void AddSeats(IEnumerable<Seat> seats) => _seats.AddRange(seats);

    public UnitResult<Error> UpdateSeats(IEnumerable<Seat> seats)
    {
        List<Seat> newSeats = seats.ToList();
        
        if (newSeats.Count() > MaxSeatsCount)
        {
            return Error.Validation("venue.seats.limit", "Too many seats");
        }

        _seats = newSeats;
        
        return UnitResult.Success<Error>();
    }

    public int SeatsCount => _seats.Count;

    public UnitResult<Error> UpdateName(string name)
    {
        var newVenueName = VenueName.Create(Name.Prefix, name);
        if (newVenueName.IsFailure)
        {
            return newVenueName.Error;
        }
        Name = newVenueName.Value;
        
        return UnitResult.Success<Error>();
    }
    
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
        int seatsLimit,
        Guid? venueId = null)
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
        
        return new Venue(
            venueId ?? Guid.NewGuid(), 
            venueNameResult.Value, 
            seatsLimit,
            []);

    }
    
    public void ExpandSeatsLimit(int newSeatsLimit) => MaxSeatsCount = newSeatsLimit;
}