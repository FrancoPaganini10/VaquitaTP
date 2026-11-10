using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vaquita.Entidades.Configuraciones
{
    public class IntegranteConfig : IEntityTypeConfiguration<Integrante>
    {
        public void Configure(EntityTypeBuilder<Integrante> builder)
        {
            builder.HasKey(i => new { i.GrupoId, i.UsuarioId });

            builder.HasOne(i => i.Grupo)
                .WithMany(g => g.Integrantes)
                .HasForeignKey(i => i.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Usuario)
                .WithMany(u => u.Integrantes)
                .HasForeignKey(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
