namespace SeatReservationDomain.Event;

public class EventDetails
{
    public EventDetails(int capacity, string description)
    {
        Capacity = capacity;
        Description = description;
    }
    
    public Guid Id { get; private set; } = Guid.Empty;
    public int Capacity { get; private set; }
    public string Description { get; private set; }
}