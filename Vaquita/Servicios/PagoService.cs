using Microsoft.EntityFrameworkCore;
using Vaquita.DTOs;
using Vaquita.Entidades;
using Vaquita.Excepciones;
using Vaquita.Utilidades;

namespace Vaquita.Servicios
{
    public class PagoService : IPagoService
    {
        private readonly ApplicationDbContext context;
        private readonly ISaldoService saldoService;

        public PagoService(ApplicationDbContext context, ISaldoService saldoService)
        {
            this.context = context;
            this.saldoService = saldoService;
        }

        public async Task<List<PagoDTO>> ObtenerPorGrupo(int grupoId, int usuarioId)
        {
            await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId);

            return await context.Pagos
                .Where(p => p.GrupoId == grupoId)
                .OrderByDescending(p => p.Fecha).ThenByDescending(p => p.Id)
                .Select(p => new PagoDTO
                {
                    Id = p.Id,
                    Fecha = p.Fecha,
                    Deudor = p.Deudor.Apodo,
                    Acreedor = p.Acreedor.Apodo,
                    Importe = p.Importe
                })
                .ToListAsync();
        }

        // Pago (parcial o total) de un integrante deudor a otro acreedor
        public async Task<int> Crear(int grupoId, int usuarioId, PagoCreacionDTO pagoCreacion)
        {
            await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId, soloAbierto: true);

            if (pagoCreacion.DeudorId == pagoCreacion.AcreedorId)
            {
                throw new ReglaDeNegocioException("El deudor y el acreedor deben ser distintos.");
            }

            if (usuarioId != pagoCreacion.DeudorId && usuarioId != pagoCreacion.AcreedorId)
            {
                throw new AccesoDenegadoException(
                    "Solo el deudor o el acreedor pueden registrar el pago.");
            }

            var integrantesValidos = await context.Integrantes.CountAsync(i =>
                i.GrupoId == grupoId &&
                (i.UsuarioId == pagoCreacion.DeudorId || i.UsuarioId == pagoCreacion.AcreedorId));

            if (integrantesValidos != 2)
            {
                throw new ReglaDeNegocioException("Ambos deben ser integrantes del grupo.");
            }

            // No se puede pagar más que lo adeudado según el Cuadro 2
            var resumen = await saldoService.Calcular(grupoId);

            var deuda = resumen.Deudas.FirstOrDefault(d =>
                d.DeudorId == pagoCreacion.DeudorId && d.AcreedorId == pagoCreacion.AcreedorId);

            if (deuda is null)
            {
                throw new ReglaDeNegocioException("No hay una deuda pendiente entre esos integrantes.");
            }

            if (pagoCreacion.Importe > deuda.Importe)
            {
                throw new ReglaDeNegocioException(
                    $"El pago supera la deuda actual ({deuda.Importe:N2}).");
            }

            var pago = new Pago
            {
                GrupoId = grupoId,
                DeudorId = pagoCreacion.DeudorId,
                AcreedorId = pagoCreacion.AcreedorId,
                Importe = pagoCreacion.Importe,
                Fecha = DateTime.Now
            };

            context.Add(pago);
            await context.SaveChangesAsync();

            return pago.Id;
        }
    }
}
