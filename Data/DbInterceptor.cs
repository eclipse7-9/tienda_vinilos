using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Diagnostics;
using Npgsql;
using Microsoft.EntityFrameworkCore;

namespace MiPrimeraAPI.Data;

public class PerformanceInterceptor : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        LogCommand(command, eventData);
        if (eventData.Duration.TotalMilliseconds > 200)
        {
            Console.WriteLine($"\x1b[33m[ALERTA DE RENDIMIENTO]\x1b[0m Consulta lenta detectada ({eventData.Duration.TotalMilliseconds}ms):");
            Console.WriteLine($"- SQL: {command.CommandText}");
        }
        return base.ReaderExecuted(command, eventData, result);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        LogCommand(command, eventData);
        return base.NonQueryExecuted(command, eventData, result);
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        LogCommand(command, eventData);
        return base.ScalarExecuted(command, eventData, result);
    }

    private void LogCommand(DbCommand command, CommandExecutedEventData eventData)
    {
        if (command.CommandText.Contains("SqlLogs")) return;

        var connectionString = eventData.Context?.Database.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString)) return;

        var cmdText = command.CommandText;
        var duration = eventData.Duration.TotalMilliseconds;

        // Fuego y olvido para no retrasar la petición principal
        Task.Run(async () =>
        {
            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand("INSERT INTO \"SqlLogs\" (\"CommandText\", \"DurationMs\", \"ExecutedAt\") VALUES (@txt, @dur, @at)", conn);
                cmd.Parameters.AddWithValue("txt", cmdText);
                cmd.Parameters.AddWithValue("dur", duration);
                cmd.Parameters.AddWithValue("at", DateTime.UtcNow);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging SQL: {ex.Message}");
            }
        });
    }
}
