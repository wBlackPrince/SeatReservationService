namespace SeatReservationDomain.Event;

public class Event
{
    public Event(Guid id, EventDetails details, Guid venueId, string name, DateTime eventDate)
    {
        Id = id;
        Details = details;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
    }
    
    public Guid Id { get; private set; }
    
    public EventDetails Details { get; set; }
    
    public Guid VenueId { get; private set; }
    public string Name { get; private set; }
    public DateTime EventDate { get; private set; }
}