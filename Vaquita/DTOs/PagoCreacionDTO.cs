using System.ComponentModel.DataAnnotations;
using Vaquita.Utilidades;

namespace Vaquita.DTOs
{
    public class PagoCreacionDTO
    {
        // Quien paga (el que debe)
        public int DeudorId { get; set; }

        // Quien recibe
        public int AcreedorId { get; set; }

        [Range(0.01, 999999999.99), MaximoDosDecimales]
        public decimal Importe { get; set; }
    }
}
