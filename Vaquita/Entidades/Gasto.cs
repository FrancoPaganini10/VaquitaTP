namespace Vaquita.Entidades
{
    public class Gasto
    {
        public int Id { get; set; }
        public decimal Importe { get; set; }
        public string Descripcion { get; set; } = null!;
        public DateTime Fecha { get; set; }

        public int GrupoId { get; set; }
        public Grupo Grupo { get; set; } = null!;

        // Quien puso la plata
        public int PagadorId { get; set; }
        public Usuario Pagador { get; set; } = null!;

        // Quien cargó el gasto en la app (puede ser otro usuario)
        public int RegistradoPorId { get; set; }
        public Usuario RegistradoPor { get; set; } = null!;

        // Opcional
        public int? RubroId { get; set; }
        public Rubro? Rubro { get; set; }
    }
}
