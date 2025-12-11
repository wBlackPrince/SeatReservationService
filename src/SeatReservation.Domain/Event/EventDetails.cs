using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Event;

public class EventDetails
{
    // Ef Core
    private EventDetails()
    {
        
    }
    
    public EventDetails(
        EventCapacity capacity, 
        EventDescription description)
    {
        Capacity = capacity;
        Description = description;
    }
    
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public Guid EventId { get; private set; }
    public EventCapacity Capacity { get; private set; }
    public EventDescription Description { get; private set; }
    
    public uint Version { get; private set; }
    
    public DateTime LastReservationUtc { get; private set; }

    public void ReserveSeat()
    {
        LastReservationUtc = DateTime.UtcNow;
    }
    
    public static Result<EventDetails, Error> Validate(
        string description,
        int capacity)
    {
        Result<EventCapacity, Error> capacityResult = EventCapacity.Create(capacity);

        if (capacityResult.IsFailure)
            return capacityResult.Error;
        
        
        Result<EventDescription, Error> descriptionResult = EventDescription.Create(description);

        if (descriptionResult.IsFailure)
            return descriptionResult.Error;
        
        
        return new EventDetails(capacityResult.Value, descriptionResult.Value);
    }
}