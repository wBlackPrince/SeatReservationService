using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Venue;

namespace SeatReservationDomain.Event;

public class Event
{
    //Ef Core
    private Event()
    {
        
    }
    
    private Event(
        EventId id, 
        VenueId venueId, 
        EventDetails details, 
        string name, 
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        IEventInfo info,
        EventType eventType)
    {
        Id = id;
        Details = details;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        StartDate = startDate;
        EndDate = endDate;
        Status = EventStatus.Planned;
        Info = info;
        Type = eventType;
    }
    
    public EventId Id { get; private set; }
    
    public EventDetails Details { get; set; }
    
    public VenueId VenueId { get; private set; }
    public string Name { get; private set; }
    public DateTime EventDate { get; private set; }
    
    public DateTime StartDate { get; private set; }
    
    public DateTime EndDate { get; private set; }
    
    public EventStatus Status { get; private set; }
    
    public EventType Type { get; private set; }

    public IEventInfo Info {get; private set;}

    public bool IsAvailableForReservation(int capacitySum) 
        => Status == EventStatus.Planned && 
           StartDate > DateTime.Now &&
           capacitySum <= Details.Capacity;


    private static Result<EventDetails, Error> Validate(
        string name,
        string description,
        int capacity,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Error.Validation("event.name", "Event name cannot be empty");
        }

        if (eventDate < DateTime.UtcNow)
        {
            return Error.Validation("event.date", "Event date cannot be in the past");
        }

        if (startDate >= endDate || startDate <= DateTime.UtcNow || endDate < DateTime.UtcNow)
        {
            return Error.Validation("event.date", "Time of event is incorrect");
        }

        if (capacity <= 0)
        {
            return Error.Validation("capacity", "Capacity must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Error.Validation("description", "Description cannot be empty");
        }
        
        return new EventDetails(capacity, description);
    }

    public static Result<Event, Error> CreateConcert(
        VenueId venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string perfomer)
    {
        var detailsResult = Validate(name, description, capacity, eventDate, startDate, endDate);

        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        if (string.IsNullOrEmpty(perfomer))
        {
            return Error.Validation("perfomer", "Perfomer cannot be empty");
        }
        
        var concertInfo = new ConcertInfo(perfomer);

        return new Event(
            new EventId(Guid.NewGuid()),
            venueId,
            detailsResult.Value,
            name,
            eventDate,
            startDate,
            endDate,
            concertInfo,
            EventType.Concert
        );

    }
    
    
    public static Result<Event, Error> CreateConference(
        VenueId venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string speaker,
        string topic)
    {
        var detailsResult = Validate(name, description, capacity, eventDate, startDate, endDate);

        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        if (string.IsNullOrEmpty(speaker))
        {
            return Error.Validation("speaker", "Speaker cannot be empty");
        }

        if (string.IsNullOrEmpty(topic))
        {
            return Error.Validation("topic", "Topic cannot be empty");
        }
        
        var conferenceInfo = new ConferenceInfo(speaker, topic);

        return new Event(
            new EventId(Guid.NewGuid()),
            venueId,
            detailsResult.Value,
            name,
            eventDate,
            startDate,
            endDate,
            conferenceInfo,
            EventType.Concert
        );
    }
    
    public static Result<Event, Error> CreateOnline(
        VenueId venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string url)
    {
        var detailsResult = Validate(name, description, capacity, eventDate, startDate, endDate);

        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        if (string.IsNullOrEmpty(url))
        {
            return Error.Validation("url", "Url cannot be empty");
        }

        
        var conferenceInfo = new OnlineInfo(url);

        return new Event(
            new EventId(Guid.NewGuid()),
            venueId,
            detailsResult.Value,
            name,
            eventDate,
            startDate,
            endDate,
            conferenceInfo,
            EventType.Concert
        );

    }
}


public enum EventStatus
{
    Planned,
    InProgress,
    Finished,
    Canceled,
}

public enum EventType{
    Concert,
    Conference,
    Online
}

public interface IEventInfo
{
    string ToString();
}

public record ConcertInfo(string Perfomer) : IEventInfo
{
    public override string ToString() => $"Performer: {Perfomer}";
}

public record ConferenceInfo(string Speaker, string Topic) : IEventInfo
{
    public override string ToString() => $"Speaker: {Speaker}, Topic: {Topic}";
}

public record OnlineInfo(string Url) : IEventInfo
{
    public override string ToString() => $"Url: {Url}";
}
