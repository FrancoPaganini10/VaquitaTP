namespace Vaquita.Entidades
{
    public enum TipoToken
    {
        Alta = 0,
        Login = 1
    }

    public class TokenVerificacion
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public TipoToken Tipo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Usado { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
