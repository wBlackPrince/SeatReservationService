using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Venue;

public class Venue
{
    private List<Seat> _seats;
    private List<Guid> _events;
    
    public Venue(Guid id, string name, int maxSeatsCount, IEnumerable<Seat> seats)
    {
        Id = id;
        Name = name;
        MaxSeatsCount = maxSeatsCount;
        _seats = seats.ToList();
    }
    
    public Guid Id { get;}
    
    public string Name { get; private set; }
    
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