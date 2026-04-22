using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.Data;

public class PerformanceInterceptor : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        LogCommand(command, eventData);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        LogCommand(command, eventData);
        return base.ScalarExecuted(command, eventData, result);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        LogCommand(command, eventData);
        return base.NonQueryExecuted(command, eventData, result);
    }

    private void LogCommand(DbCommand command, CommandExecutedEventData eventData)
    {
        // Evitar bucle infinito al loguear la inserción de logs
        if (command.CommandText.Contains("\"SqlLogs\"") || command.CommandText.Contains("SqlLogs"))
        {
            return;
        }

        try
        {
            if (command.Connection == null) return;

            using (var logCommand = command.Connection.CreateCommand())
            {
                logCommand.CommandText = "INSERT INTO \"SqlLogs\" (\"CommandText\", \"DurationMs\", \"ExecutedAt\") VALUES (@text, @duration, @at)";
                
                var pText = logCommand.CreateParameter();
                pText.ParameterName = "@text";
                pText.Value = command.CommandText;
                logCommand.Parameters.Add(pText);

                var pDuration = logCommand.CreateParameter();
                pDuration.ParameterName = "@duration";
                pDuration.Value = eventData.Duration.TotalMilliseconds;
                logCommand.Parameters.Add(pDuration);

                var pAt = logCommand.CreateParameter();
                pAt.ParameterName = "@at";
                pAt.Value = DateTime.UtcNow;
                logCommand.Parameters.Add(pAt);

                if (logCommand.Connection != null && logCommand.Connection.State != System.Data.ConnectionState.Open)
                {
                    logCommand.Connection.Open();
                }

                logCommand.ExecuteNonQuery();
            }
        }
        catch
        {
            // Fallback silencioso para no romper la ejecución principal
        }
    }
}
