namespace MiPrimeraAPI.DTOs.MetodoPago
{
    public class MetodoPagoCreateDto
    {
        public string Tipo { get; set; } = string.Empty;
        public string? NombreTitular { get; set; }
        public string? UltimosDigitos { get; set; }
        public string? Banco { get; set; }
        public bool EsPrincipal { get; set; }
        public int ClienteId { get; set; }
    }
}