using System.Data;
using CSharpFunctionalExtensions;
using SeatReservation.Shared;

namespace SeatReservationService.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        IsolationLevel? isolationLevel, 
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken token);
}