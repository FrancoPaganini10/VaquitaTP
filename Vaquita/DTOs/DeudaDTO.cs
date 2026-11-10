namespace Vaquita.DTOs
{
    // Fila del Cuadro 2: "Mario > Pedro 25.950$"
    public class DeudaDTO
    {
        public int DeudorId { get; set; }
        public string Deudor { get; set; } = null!;
        public int AcreedorId { get; set; }
        public string Acreedor { get; set; } = null!;
        public decimal Importe { get; set; }
    }
}
