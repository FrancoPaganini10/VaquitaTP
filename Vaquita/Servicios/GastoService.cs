using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Vaquita.DTOs;
using Vaquita.Entidades;
using Vaquita.Excepciones;
using Vaquita.Utilidades;

namespace Vaquita.Servicios
{
    public class GastoService : IGastoService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GastoService(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        // Todos los gastos del grupo: usuario, detalle e importe
        public async Task<List<GastoDTO>> ObtenerPorGrupo(int grupoId, int usuarioId)
        {
            await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId);

            return await context.Gastos
                .Where(g => g.GrupoId == grupoId && !g.Anulado)
                .OrderByDescending(g => g.Fecha).ThenByDescending(g => g.Id)
                .Select(g => new GastoDTO
                {
                    Id = g.Id,
                    Fecha = g.Fecha,
                    PagadorId = g.PagadorId,
                    Pagador = g.Pagador.Apodo,
                    Descripcion = g.Descripcion,
                    Importe = g.Importe,
                    Rubro = g.Rubro != null ? g.Rubro.Nombre : null,
                    RegistradoPor = g.RegistradoPor.Apodo
                })
                .ToListAsync();
        }

        // Ej.: cuánto gastó el grupo en Combustible
        public async Task<List<TotalRubroDTO>> ObtenerTotalesPorRubro(int grupoId, int usuarioId)
        {
            await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId);

            return await context.Gastos
                .Where(g => g.GrupoId == grupoId && !g.Anulado)
                .GroupBy(g => g.Rubro != null ? g.Rubro.Nombre : "Sin rubro")
                .Select(x => new TotalRubroDTO { Rubro = x.Key, Total = x.Sum(g => g.Importe) })
                .OrderByDescending(x => x.Total)
                .ToListAsync();
        }

        public async Task<int> Crear(int grupoId, int usuarioId, GastoCreacionDTO gastoCreacion)
        {
            await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId, soloAbierto: true);

            // Un usuario puede registrar un gasto propio o de otro integrante
            var pagadorId = gastoCreacion.PagadorId ?? usuarioId;

            var pagadorEsIntegrante = await context.Integrantes
                .AnyAsync(i => i.GrupoId == grupoId && i.UsuarioId == pagadorId);

            if (!pagadorEsIntegrante)
            {
                throw new ReglaDeNegocioException("El pagador no es integrante del grupo.");
            }

            if (gastoCreacion.RubroId is not null)
            {
                var existeRubro = await context.Rubros.AnyAsync(r => r.Id == gastoCreacion.RubroId);

                if (!existeRubro)
                {
                    throw new ReglaDeNegocioException("El rubro indicado no existe.");
                }
            }

            var gasto = mapper.Map<Gasto>(gastoCreacion);
            gasto.GrupoId = grupoId;
            gasto.PagadorId = pagadorId;
            gasto.RegistradoPorId = usuarioId;
            gasto.Fecha = Reloj.Hoy();

            context.Add(gasto);
            await context.SaveChangesAsync();

            return gasto.Id;
        }

        public async Task Anular(int grupoId, int gastoId, int usuarioId)
        {
            var grupo = await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId, soloAbierto: true);

            await using var transaccion = await context.Database.BeginTransactionAsync();
            await context.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT Id FROM Grupos WITH (UPDLOCK, ROWLOCK) WHERE Id = {grupoId}");

            var gasto = await context.Gastos
                .FirstOrDefaultAsync(g => g.Id == gastoId && g.GrupoId == grupoId)
                ?? throw new NoEncontradoException("El gasto no existe.");

            if (gasto.Anulado)
                throw new ReglaDeNegocioException("El gasto ya está anulado.");

            // Solo quien pagó el gasto o el administrador
            if (usuarioId != gasto.PagadorId && usuarioId != grupo.AdministradorId)
                throw new AccesoDenegadoException("No podés anular este gasto.");

            gasto.Anulado = true;
            gasto.FechaAnulacion = DateTime.UtcNow;
            gasto.AnuladoPorId = usuarioId;

            await context.SaveChangesAsync();
            await transaccion.CommitAsync();
        }
    }
}
