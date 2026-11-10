using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vaquita.Entidades.Configuraciones
{
    public class GrupoConfig : IEntityTypeConfiguration<Grupo>
    {
        public void Configure(EntityTypeBuilder<Grupo> builder)
        {
            builder.Property(g => g.Descripcion).HasMaxLength(200);

            builder.HasOne(g => g.Administrador)
                .WithMany()
                .HasForeignKey(g => g.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
