namespace Vaquita.Entidades
{
    // Pago entre integrantes para saldar deudas (parcial o total)
    public class Pago
    {
        public int Id { get; set; }
        public decimal Importe { get; set; }
        public DateTime Fecha { get; set; }

        public int GrupoId { get; set; }
        public Grupo Grupo { get; set; } = null!;

        // Quien paga (el que debía)
        public int DeudorId { get; set; }
        public Usuario Deudor { get; set; } = null!;

        // Quien recibe
        public int AcreedorId { get; set; }
        public Usuario Acreedor { get; set; } = null!;
    }
}
