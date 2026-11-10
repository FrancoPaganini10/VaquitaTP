using System.ComponentModel.DataAnnotations;

namespace Vaquita.DTOs
{
    public class SolicitudAccesoDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
    }
}
