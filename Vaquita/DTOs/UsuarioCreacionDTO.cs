using System.ComponentModel.DataAnnotations;

namespace Vaquita.DTOs
{
    public class UsuarioCreacionDTO
    {
        [Required, StringLength(100)]
        public string Apellido { get; set; } = null!;

        [Required, StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, StringLength(50)]
        public string Apodo { get; set; } = null!;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = null!;
    }
}
