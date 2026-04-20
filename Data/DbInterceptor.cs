using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Diagnostics;

namespace MiPrimeraAPI.Data;

public class PerformanceInterceptor : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        if (eventData.Duration.TotalMilliseconds > 200)
        {
            Console.WriteLine($"\x1b[33m[ALERTA DE RENDIMIENTO]\x1b[0m Consulta lenta detectada ({eventData.Duration.TotalMilliseconds}ms):");
            Console.WriteLine($"- SQL: {command.CommandText}");
        }
        return base.ReaderExecuted(command, eventData, result);
    }
}
