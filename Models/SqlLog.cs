using System;

namespace MiPrimeraAPI.Models
{
    public class SqlLog
    {
        public int Id { get; set; }
        public string CommandText { get; set; } = string.Empty;
        public double DurationMs { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }
}
