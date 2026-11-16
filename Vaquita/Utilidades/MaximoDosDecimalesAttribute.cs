using System.ComponentModel.DataAnnotations;

namespace Vaquita.Utilidades
{
    // Rechaza importes con más de 2 decimales (la base guarda decimal(18,2))
    public class MaximoDosDecimalesAttribute : ValidationAttribute
    {
        public MaximoDosDecimalesAttribute()
            : base("El importe no puede tener más de 2 decimales.")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is not decimal importe)
            {
                return true;
            }

            return decimal.Round(importe, 2) == importe;
        }
    }
}
