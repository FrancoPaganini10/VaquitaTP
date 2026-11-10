using System.ComponentModel.DataAnnotations;

namespace Vaquita.DTOs
{
    public class VerificarTokenDTO
    {
        [Required]
        public string Token { get; set; } = null!;
    }
}
