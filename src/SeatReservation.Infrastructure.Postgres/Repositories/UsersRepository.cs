using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationDomain;
using SeatReservationDomain.Event;
using SeatReservationDomain.Users;
using SeatReservationService.Application.Users;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class UsersRepository : IUsersRepository
{
    ReservationServiceDbContext _dbContext;
    ILogger<EventsRepository> _logger;

    public UsersRepository(
        ReservationServiceDbContext dbContext,
        ILogger<UsersRepository> logger)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Result<Guid, Error>> Create(User user, CancellationToken cancellationToken){
        await _dbContext.Users.AddAsync(user, cancellationToken);
        
        return user.Id;
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}