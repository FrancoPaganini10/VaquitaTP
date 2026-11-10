namespace Vaquita.DTOs
{
    public class PagoDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Deudor { get; set; } = null!;
        public string Acreedor { get; set; } = null!;
        public decimal Importe { get; set; }
    }
}
