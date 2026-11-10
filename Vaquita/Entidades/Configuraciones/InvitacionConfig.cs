using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vaquita.Entidades.Configuraciones
{
    public class InvitacionConfig : IEntityTypeConfiguration<Invitacion>
    {
        public void Configure(EntityTypeBuilder<Invitacion> builder)
        {
            builder.Property(i => i.Email).HasMaxLength(200);
            builder.HasIndex(i => i.Token).IsUnique();

            builder.HasOne(i => i.Grupo)
                .WithMany(g => g.Invitaciones)
                .HasForeignKey(i => i.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.InvitadoPor)
                .WithMany()
                .HasForeignKey(i => i.InvitadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
