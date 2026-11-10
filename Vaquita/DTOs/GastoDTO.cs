namespace Vaquita.DTOs
{
    public class GastoDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int PagadorId { get; set; }
        public string Pagador { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal Importe { get; set; }
        public string? Rubro { get; set; }
        public string RegistradoPor { get; set; } = null!;
    }
}
