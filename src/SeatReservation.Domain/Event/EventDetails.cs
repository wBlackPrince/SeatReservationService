namespace SeatReservationDomain.Event;


public record EventDetailsId(Guid Value);

public class EventDetails
{
    // Ef Core
    private EventDetails()
    {
        
    }
    
    public EventDetails(
        int capacity, 
        string description)
    {
        Capacity = capacity;
        Description = description;
    }
    
    public EventDetailsId Id { get; private set; } = new EventDetailsId(Guid.NewGuid());
    
    public EventId EventId { get; private set; }
    public int Capacity { get; private set; }
    public string Description { get; private set; }
}