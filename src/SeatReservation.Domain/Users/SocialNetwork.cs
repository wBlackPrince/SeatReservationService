using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationDomain.Users;

public record SocialNetwork
{
    private SocialNetwork()
    {
    }
    
    private SocialNetwork(SocialNetworkName name, SocialNetworkLink link)
    {
        Name = name;
        Link = link;
    }
    
    public SocialNetworkName Name { get; set; }
    public SocialNetworkLink Link { get; set;  }

    public static Result<SocialNetwork, Error> Create(string name, string link)
    {
        Result<SocialNetworkName, Error> socialNetworkNameResult = SocialNetworkName.Create(name);

        if (socialNetworkNameResult.IsFailure)
            return socialNetworkNameResult.Error;
        
        Result<SocialNetworkLink, Error> socialNetworkLinkResult = SocialNetworkLink.Create(link);

        if (socialNetworkLinkResult.IsFailure)
            return socialNetworkLinkResult.Error;
        
        return new SocialNetwork(socialNetworkNameResult.Value, socialNetworkLinkResult.Value);
    }
}