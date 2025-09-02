using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SeatReservation.Infrastructure.Postgres;

public class SlowQueryInterceptor : DbCommandInterceptor
{
    private const int _slowQueryThreshold = 200; // миллисекунды

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        if (eventData.Duration.TotalMilliseconds > _slowQueryThreshold)
        {
            // Логируем медленный запрос — тут можно вставить свою систему логирования
            Console.WriteLine(
                $"Slow query ({eventData.Duration.TotalMilliseconds} ms): {command.CommandText}");
        }

        return base.ReaderExecuted(command, eventData, result);
    }
}