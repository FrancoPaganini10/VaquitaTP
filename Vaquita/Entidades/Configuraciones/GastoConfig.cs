using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vaquita.Entidades.Configuraciones
{
    public class GastoConfig : IEntityTypeConfiguration<Gasto>
    {
        public void Configure(EntityTypeBuilder<Gasto> builder)
        {
            builder.Property(g => g.Importe).HasPrecision(18, 2);
            builder.Property(g => g.Descripcion).HasMaxLength(200);
            builder.Property(g => g.Fecha).HasColumnType("date");

            builder.ToTable(t => t.HasCheckConstraint("CK_Gasto_Importe", "[Importe] > 0"));

            builder.HasOne(g => g.Grupo)
                .WithMany(gr => gr.Gastos)
                .HasForeignKey(g => g.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.Pagador)
                .WithMany(u => u.GastosPagados)
                .HasForeignKey(g => g.PagadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.RegistradoPor)
                .WithMany()
                .HasForeignKey(g => g.RegistradoPorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.Rubro)
                .WithMany(r => r.Gastos)
                .HasForeignKey(g => g.RubroId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
