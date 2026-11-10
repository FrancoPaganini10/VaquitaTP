namespace Vaquita.DTOs
{
    public class GrupoDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public int AdministradorId { get; set; }
        public List<IntegranteDTO> Integrantes { get; set; } = new List<IntegranteDTO>();
    }
}
