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
        Guid id, 
        Guid venueId, 
        EventDetails details, 
        EventName name, 
        EventDates dates,
        IEventInfo info,
        EventType eventType)
    {
        Id = id;
        Details = details;
        VenueId = venueId;
        Name = name;
        Dates = dates;
        Status = EventStatus.Planned;
        Info = info;
        Type = eventType;
    }
    
    public Guid Id { get; private set; }
    
    public EventDetails Details { get; set; }
    
    public Guid VenueId { get; private set; }
    public EventName Name { get; private set; }
    
    public EventDates Dates { get; private set; }
    
    public EventStatus Status { get; private set; }
    
    public EventType Type { get; private set; }

    public IEventInfo Info {get; private set;}

    public bool IsAvailableForReservation(int capacitySum) 
        => Status == EventStatus.Planned && 
           Dates.StartDate > DateTime.Now &&
           capacitySum <= Details.Capacity.Value;

    public static Result<Event, Error> CreateConcert(
        Guid venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string perfomer)
    {
        var detailsResult = EventDetails.Validate(description, capacity);

        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        
        if (string.IsNullOrEmpty(perfomer))
        {
            return Error.Validation("perfomer", "Perfomer cannot be empty");
        }
        
        var concertInfo = new ConcertInfo(perfomer);
        
        Result<EventName, Error> nameResult = EventName.Create(name);

        if (nameResult.IsFailure)
            return nameResult.Error;
        
        Result<EventDates, Error> eventDatesResult = EventDates.Create(eventDate, startDate, endDate);

        if (eventDatesResult.IsFailure)
            return eventDatesResult.Error;
        
        

        return new Event(
            Guid.NewGuid(),
            venueId,
            detailsResult.Value,
            nameResult.Value,
            eventDatesResult.Value,
            concertInfo,
            EventType.Concert
        );

    }
    
    
    public static Result<Event, Error> CreateConference(
        Guid venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string speaker,
        string topic)
    {
        var detailsResult = EventDetails.Validate(description, capacity);

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
        
        
        
        Result<EventName, Error> nameResult = EventName.Create(name);

        if (nameResult.IsFailure)
            return nameResult.Error;
        
        Result<EventDates, Error> eventDatesResult = EventDates.Create(eventDate, startDate, endDate);

        if (eventDatesResult.IsFailure)
            return eventDatesResult.Error;

        
        
        return new Event(
            Guid.NewGuid(),
            venueId,
            detailsResult.Value,
            nameResult.Value,
            eventDatesResult.Value,
            conferenceInfo,
            EventType.Conference
        );
    }
    
    public static Result<Event, Error> CreateOnline(
        Guid venueId,
        string name,
        DateTime eventDate,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string description,
        string url)
    {
        var detailsResult = EventDetails.Validate(description, capacity);

        if (detailsResult.IsFailure)
        {
            return detailsResult.Error;
        }

        if (string.IsNullOrEmpty(url))
        {
            return Error.Validation("url", "Url cannot be empty");
        }

        
        var conferenceInfo = new OnlineInfo(url);
        
        
        Result<EventName, Error> nameResult = EventName.Create(name);

        if (nameResult.IsFailure)
            return nameResult.Error;
        
        Result<EventDates, Error> eventDatesResult = EventDates.Create(eventDate, startDate, endDate);

        if (eventDatesResult.IsFailure)
            return eventDatesResult.Error;
        
        

        return new Event(
            Guid.NewGuid(),
            venueId,
            detailsResult.Value,
            nameResult.Value,
            eventDatesResult.Value,
            conferenceInfo,
            EventType.Online
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
