using System.ComponentModel.DataAnnotations;

namespace Vaquita.DTOs
{
    public class GrupoCreacionDTO
    {
        [Required, StringLength(200)]
        public string Descripcion { get; set; } = null!;
    }
}
