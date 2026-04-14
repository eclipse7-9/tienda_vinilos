using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Diagnostics;

namespace MiPrimeraAPI.Data;

public class PerformanceInterceptor : DbCommandInterceptor
{
    // Se ejecuta antes de la consulta para iniciar un cronómetro
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        return base.ReaderExecuting(command, eventData, result);
    }

    // Se ejecuta al terminar la consulta
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        // Si la consulta tarda más de 200ms, lanzamos un aviso
        if (eventData.Duration.TotalMilliseconds > 200)
        {
            Console.WriteLine($"\x1b[33m[ALERTA DE RENDIMIENTO]\x1b[0m Consulta lenta detectada ({eventData.Duration.TotalMilliseconds}ms):");
            Console.WriteLine($"- SQL: {command.CommandText}");
        }
        return base.ReaderExecuted(command, eventData, result);
    }
}