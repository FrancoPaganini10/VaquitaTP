namespace Vaquita.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Apodo { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
