using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vaquita.Entidades.Configuraciones
{
    public class PagoConfig : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.Property(p => p.Importe).HasPrecision(18, 2);
            builder.Property(p => p.Fecha).HasColumnType("date");

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Pago_Importe", "[Importe] > 0");
                t.HasCheckConstraint("CK_Pago_Personas", "[DeudorId] <> [AcreedorId]");
            });

            builder.HasOne(p => p.Grupo)
                .WithMany(g => g.Pagos)
                .HasForeignKey(p => p.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Deudor)
                .WithMany()
                .HasForeignKey(p => p.DeudorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Acreedor)
                .WithMany()
                .HasForeignKey(p => p.AcreedorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
