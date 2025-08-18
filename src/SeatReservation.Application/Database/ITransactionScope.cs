using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservation.Infrastructure.Postgres.Database;

public interface ITransactionScope: IDisposable
{
    public UnitResult<Error> Commit();

    public UnitResult<Error> Rollback();
}