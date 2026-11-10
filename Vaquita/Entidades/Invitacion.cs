namespace Vaquita.Entidades
{
    public enum EstadoInvitacion
    {
        Pendiente = 0,
        Aceptada = 1,
        Vencida = 2
    }

    public class Invitacion
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public EstadoInvitacion Estado { get; set; } = EstadoInvitacion.Pendiente;
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaVencimiento { get; set; }

        public int GrupoId { get; set; }
        public Grupo Grupo { get; set; } = null!;

        public int InvitadoPorId { get; set; }
        public Usuario InvitadoPor { get; set; } = null!;
    }
}
