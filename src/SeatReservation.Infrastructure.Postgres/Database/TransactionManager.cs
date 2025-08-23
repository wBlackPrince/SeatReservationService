using System.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SeatReservation.Shared;
using SeatReservationService.Application.Database;

namespace SeatReservation.Infrastructure.Postgres.Database;

public class TransactionManager : ITransactionManager
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(
        ReservationServiceDbContext dbContext,
        ILogger<TransactionManager> logger,
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }
    
    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        IsolationLevel? isolationLevel = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync(
                isolationLevel ?? IsolationLevel.ReadCommitted, 
                cancellationToken);
            
            var logger = _loggerFactory.CreateLogger<TransactionScope>();

            var transactionScope = new TransactionScope(transaction.GetDbTransaction(), logger);

            return transactionScope;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "BeginTransactionAsync failed");
            return Error.Failure("transaction", "Failed to begin transaction");
        }
    }
    
    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken token)
    {
        try
        {
            await _dbContext.SaveChangesAsync(token);

            return UnitResult.Success<Error>();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "SaveChangeAsync failed");
            return Error.Failure("transaction", "Failed to save changes");
        }
    }
}