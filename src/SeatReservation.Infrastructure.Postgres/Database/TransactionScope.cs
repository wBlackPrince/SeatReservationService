using System.Data;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationService.Application.Database;

namespace SeatReservation.Infrastructure.Postgres.Database;

public class TransactionScope: ITransactionScope
{
    private readonly IDbTransaction _transaction;
    private readonly ILogger<TransactionScope> _logger;

    public TransactionScope(
        IDbTransaction transaction, 
        ILogger<TransactionScope> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public UnitResult<Error> Commit()
    {
        try
        {
            _transaction.Commit();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed commit transaction");
            return Error.Failure("transaction","Failed to commit transaction");
        }
    }

    public UnitResult<Error> Rollback()
    {
        try
        {
            _transaction.Rollback();
            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed rollback transaction");
            return Error.Failure("transaction", "Failed to rollback transaction");
        }
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}