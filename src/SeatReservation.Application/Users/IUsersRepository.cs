using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain.Users;

namespace SeatReservationService.Application.Users;

public interface IUsersRepository
{
    Task<Result<Guid, Error>> Create(User user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}