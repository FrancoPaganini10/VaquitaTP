using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vaquita.Entidades.Configuraciones
{
    public class RubroConfig : IEntityTypeConfiguration<Rubro>
    {
        public void Configure(EntityTypeBuilder<Rubro> builder)
        {
            builder.HasData(
                new Rubro { Id = 1, Nombre = "Combustible" },
                new Rubro { Id = 2, Nombre = "Alojamiento" },
                new Rubro { Id = 3, Nombre = "Transporte" },
                new Rubro { Id = 4, Nombre = "Materiales" },
                new Rubro { Id = 5, Nombre = "Mano de obra" }
            );

            builder.HasIndex(r => r.Nombre).IsUnique();
        }
    }
}
