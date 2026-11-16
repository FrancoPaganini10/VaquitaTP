using System.ComponentModel.DataAnnotations;
using Vaquita.Utilidades;

namespace Vaquita.DTOs
{
    public class GastoCreacionDTO
    {
        [Range(0.01, 999999999.99), MaximoDosDecimales]
        public decimal Importe { get; set; }

        [Required, StringLength(200)]
        public string Descripcion { get; set; } = null!;

        // Opcional: si no se informa, el rubro queda sin especificar
        public int? RubroId { get; set; }

        // Opcional: si no se informa, el pagador es el usuario logueado.
        // Permite registrar un gasto de otro integrante.
        public int? PagadorId { get; set; }
    }
}
