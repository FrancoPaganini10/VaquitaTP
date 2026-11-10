namespace Vaquita.Entidades
{
    // Tabla intermedia Usuario <-> Grupo (muchos a muchos sin salto, como PeliculaActor)
    public class Integrante
    {
        public int GrupoId { get; set; }
        public Grupo Grupo { get; set; } = null!;
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public bool EsAdministrador { get; set; }
        public DateTime FechaIngreso { get; set; }
        // Permite abrir la app en el último grupo en el que se trabajó
        public DateTime UltimoAcceso { get; set; }
    }
}
