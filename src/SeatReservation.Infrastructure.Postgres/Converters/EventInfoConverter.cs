using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeatReservationDomain.Event;

namespace SeatReservation.Infrastructure.Postgres.Converters;

public class EventInfoConverter : ValueConverter<IEventInfo, string>
{
    public EventInfoConverter() : base(i => InfoToString(i), s => StringToInfo(s))
    {
        
    }
    
    private static string InfoToString(IEventInfo info) => info switch
    {
        ConcertInfo c => $"Concert:{c.Perfomer}",
        ConferenceInfo c => $"Conference:{c.Speaker}|{c.Topic}",
        OnlineInfo o => $"Online:{o.Url}",
        _ => throw new ArgumentOutOfRangeException("Unknown type of info")
    };
    
    private static IEventInfo StringToInfo(string info)
    {
        var split = info.Split(':');
        string type = split[0];
        string data = split[1];

        return type switch
        {
            "Concert" => new ConcertInfo(data),
            "Conference" => new ConferenceInfo(data.Split('|')[0], data.Split('|')[1]),
            "Online" => new OnlineInfo(data),
            _ => throw new ArgumentOutOfRangeException("Unknown type of info")
        };
    }
}