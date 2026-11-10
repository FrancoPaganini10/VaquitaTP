using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vaquita.Entidades.Configuraciones
{
    public class TokenVerificacionConfig : IEntityTypeConfiguration<TokenVerificacion>
    {
        public void Configure(EntityTypeBuilder<TokenVerificacion> builder)
        {
            builder.HasIndex(t => t.Token).IsUnique();

            builder.HasOne(t => t.Usuario)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
