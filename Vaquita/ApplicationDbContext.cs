using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vaquita.Entidades;

namespace Vaquita
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Grupo>()
        .HasIndex(g => g.CodigoInvitacion)
        .IsUnique();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>().HaveMaxLength(150);
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Grupo> Grupos => Set<Grupo>();
        public DbSet<Integrante> Integrantes => Set<Integrante>();
        public DbSet<Rubro> Rubros => Set<Rubro>();
        public DbSet<Gasto> Gastos => Set<Gasto>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<Invitacion> Invitaciones => Set<Invitacion>();
        public DbSet<TokenVerificacion> TokensVerificacion => Set<TokenVerificacion>();
    }
}
