namespace Vaquita.DTOs
{
    // Cuadro 1 + Cuadro 2 en una sola respuesta
    public class ResumenSaldosDTO
    {
        public decimal TotalGrupo { get; set; }
        public decimal CuotaPorIntegrante { get; set; }
        public bool Saldado { get; set; }
        public List<SaldoIntegranteDTO> Saldos { get; set; } = new List<SaldoIntegranteDTO>();
        public List<DeudaDTO> Deudas { get; set; } = new List<DeudaDTO>();
    }
}
