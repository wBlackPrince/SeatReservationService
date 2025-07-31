using SeatReservationDomain.Venue;

namespace SeatReservationDomain.Event;

public record EventId(Guid Value);

public class Event
{
    //Ef Core
    private Event()
    {
        
    }
    
    public Event(
        EventId id, 
        EventDetails details, 
        VenueId venueId, 
        string name, 
        DateTime eventDate,
        IEventInfo info)
    {
        Id = id;
        Details = details;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        Info = info;
    }
    
    public EventId Id { get; private set; }
    
    public EventDetails Details { get; set; }
    
    public VenueId VenueId { get; private set; }
    public string Name { get; private set; }
    public DateTime EventDate { get; private set; }
    
    public EventType Type { get; private set; }

    public IEventInfo Info {get; private set;}
}



public enum EventType{
    Concert,
    Conference,
    Online
}

public interface IEventInfo{

}

public record ConcertInfo(string Perfomer): IEventInfo;
public record ConferenceInfo(string Speaker, string Topic): IEventInfo;
public record OnlineInfo(string Url): IEventInfo;
