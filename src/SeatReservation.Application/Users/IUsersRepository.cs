using CSharpFunctionalExtensions;
using SeatReservation.Shared;
using SeatReservationDomain;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public interface IUsersRepository
{
    Task<Result<Guid, Error>> Create(User user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}