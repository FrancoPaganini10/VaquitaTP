using Microsoft.EntityFrameworkCore;
using Vaquita.DTOs;

namespace Vaquita.Servicios
{
    public class RubroService : IRubroService
    {
        private readonly ApplicationDbContext context;

        public RubroService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<RubroDTO>> ObtenerTodos() =>
            await context.Rubros
                .OrderBy(r => r.Nombre)
                .Select(r => new RubroDTO { Id = r.Id, Nombre = r.Nombre })
                .ToListAsync();
    }
}
