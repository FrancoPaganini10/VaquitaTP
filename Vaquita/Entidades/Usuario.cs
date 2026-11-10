namespace Vaquita.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Apellido { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apodo { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool EmailVerificado { get; set; }
        public DateTime FechaAlta { get; set; }

        public List<Integrante> Integrantes { get; set; } = new List<Integrante>();
        public List<Gasto> GastosPagados { get; set; } = new List<Gasto>();
        public List<TokenVerificacion> Tokens { get; set; } = new List<TokenVerificacion>();
    }
}
