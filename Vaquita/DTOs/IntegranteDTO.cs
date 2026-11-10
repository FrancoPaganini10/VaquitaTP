namespace Vaquita.DTOs
{
    public class IntegranteDTO
    {
        public int UsuarioId { get; set; }
        public string Apodo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public bool EsAdministrador { get; set; }
    }
}
