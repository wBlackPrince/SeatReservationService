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
    
    public VenueName Name { get; private set; }
    
    public int MaxSeatsCount {get; private set;}
    
    public IReadOnlyCollection<Seat> Seats => _seats;
    public IReadOnlyList<Guid> Events => _events;
    
    public void AddEvent(Guid @event) => _events.Add(@event); 
    
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
    
    public void ExpandSeatsLimit(int newSeatsLimit) => MaxSeatsCount = newSeatsLimit;
}