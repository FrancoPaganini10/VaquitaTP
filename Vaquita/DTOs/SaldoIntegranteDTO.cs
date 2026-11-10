namespace Vaquita.DTOs
{
    // Fila del Cuadro 1
    public class SaldoIntegranteDTO
    {
        public int UsuarioId { get; set; }
        public string Apodo { get; set; } = null!;
        public decimal TotalGastos { get; set; }
        public decimal Saldo { get; set; }
    }
}
