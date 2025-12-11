

namespace SeatReservationDomain.Users;

public class User
{

    // ef core
    private User()
    {
        
    }
    
    public User(Details details)
    {
        Details = details;
    }
    
    public Guid Id { get; set; }

    public Details Details { get; set; }
}