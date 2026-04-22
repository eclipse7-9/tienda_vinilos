using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Diagnostics;

namespace MiPrimeraAPI.Data;

public class PerformanceInterceptor : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        LogToDb(command, eventData.Duration.TotalMilliseconds);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        LogToDb(command, eventData.Duration.TotalMilliseconds);
        return base.ScalarExecuted(command, eventData, result);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        LogToDb(command, eventData.Duration.TotalMilliseconds);
        return base.NonQueryExecuted(command, eventData, result);
    }

    private void LogToDb(DbCommand command, double durationMs)
    {
        var sql = command.CommandText;
        if (sql.Contains("SqlLogs") || sql.Contains("INSERT INTO") && sql.Contains("@text")) return;

        Task.Run(async () => {
            try 
            {
                // Usamos una conexión nueva para no interferir con la transacción actual
                // Obtenemos el connection string del comando original
                var connString = command.Connection?.ConnectionString;
                if (string.IsNullOrEmpty(connString)) return;

                using var conn = new Npgsql.NpgsqlConnection(connString);
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO \"SqlLogs\" (\"CommandText\", \"DurationMs\", \"ExecutedAt\") VALUES (@t, @d, @a)";
                
                var p1 = cmd.CreateParameter(); p1.ParameterName = "@t"; p1.Value = sql; cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@d"; p2.Value = durationMs; cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter(); p3.ParameterName = "@a"; p3.Value = DateTime.UtcNow; cmd.Parameters.Add(p3);
                
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                // Si falla, al menos lo vemos en la consola del server
                Console.WriteLine($"[Interceptor Error] {ex.Message}");
            }
        });
    }
}
