using System.ComponentModel.DataAnnotations;

namespace Vaquita.DTOs
{
    public class InvitacionCreacionDTO
    {
        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = null!;
    }
}
