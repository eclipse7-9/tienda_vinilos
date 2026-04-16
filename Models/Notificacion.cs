using System;

namespace MiPrimeraAPI.Models
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public bool Leida { get; set; } = false;
        
        public Cliente Cliente { get; set; } = null!;
    }
}
