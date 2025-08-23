using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationService.Application.Database;

public interface ITransactionScope: IDisposable
{
    public UnitResult<Error> Commit();

    public UnitResult<Error> Rollback();
}