namespace Vaquita.DTOs
{
    // Para la lista desplegable de grupos activos
    public class GrupoListadoDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;
        public int CantidadIntegrantes { get; set; }
    }
}
