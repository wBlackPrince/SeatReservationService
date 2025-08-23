namespace SeatReservationDomain.Users;

public class User
{

    // ef core
    public User()
    {
        
    }
    
    public User(Details details)
    {
        Details = details;
    }
    
    public Guid Id { get; set; }

    public Details Details { get; set; }
}


public record Details
{
    private List<SocialNetwork> _socials;

    public Details()
    {

    }

    public string Description { get; set; }
    public string FIO { get; set; }

    public IReadOnlyList<SocialNetwork> Socials
    {
        get { return _socials; }
        set { _socials = value.ToList(); }
    }
}

public record SocialNetwork
{
    public SocialNetwork()
    {
        
    }
    
    public string Name { get; set; }
    public string Link { get; set;  }
}