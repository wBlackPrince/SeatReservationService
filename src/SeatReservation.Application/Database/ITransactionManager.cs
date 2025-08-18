using System.Transactions;
using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservation.Infrastructure.Postgres.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken token);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken token);
}