namespace Vaquita.Entidades
{
    public class Grupo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string CodigoInvitacion { get; set; } = null!;

        public int AdministradorId { get; set; }
        public Usuario Administrador { get; set; } = null!;

        public List<Integrante> Integrantes { get; set; } = new List<Integrante>();
        public List<Gasto> Gastos { get; set; } = new List<Gasto>();
        public List<Pago> Pagos { get; set; } = new List<Pago>();
        public List<Invitacion> Invitaciones { get; set; } = new List<Invitacion>();
    }
}
