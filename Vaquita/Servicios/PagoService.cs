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
                .Where(p => p.GrupoId == grupoId && !p.Anulado)
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

            await using var transaccion = await context.Database.BeginTransactionAsync();          
            await context.Database.ExecuteSqlInterpolatedAsync($"SELECT Id FROM dbo.Grupos WITH (UPDLOCK, ROWLOCK) WHERE Id = {grupoId}");

// El deudor debe tener deuda, el acreedor saldo a favor, y el pago no puede superar el mínimo de ambos
var resumen = await saldoService.Calcular(grupoId);

var saldoDeudor = resumen.Saldos.FirstOrDefault(s => s.UsuarioId == pagoCreacion.DeudorId)?.Saldo ?? 0;
var saldoAcreedor = resumen.Saldos.FirstOrDefault(s => s.UsuarioId == pagoCreacion.AcreedorId)?.Saldo ?? 0;

if (saldoDeudor >= 0)
{
    throw new ReglaDeNegocioException("El deudor no tiene deuda pendiente.");
}

if (saldoAcreedor <= 0)
{
    throw new ReglaDeNegocioException("El acreedor no tiene saldo a favor.");
}

var maximo = Math.Round(Math.Min(-saldoDeudor, saldoAcreedor), 2);

if (pagoCreacion.Importe > maximo)
{
    throw new ReglaDeNegocioException(
        $"El pago supera el máximo permitido ({maximo:N2}).");
}

            var pago = new Pago
            {
                GrupoId = grupoId,
                DeudorId = pagoCreacion.DeudorId,
                AcreedorId = pagoCreacion.AcreedorId,
                Importe = pagoCreacion.Importe,
                Fecha = Reloj.Hoy(),
            };

            context.Add(pago);
            await context.SaveChangesAsync();
            await transaccion.CommitAsync();          

            return pago.Id;
        }

        public async Task Anular(int grupoId, int pagoId, int usuarioId)
        {
    var grupo = await context.ObtenerGrupoComoIntegrante(grupoId, usuarioId, soloAbierto: true);

    await using var transaccion = await context.Database.BeginTransactionAsync();
    await context.Database.ExecuteSqlInterpolatedAsync(
        $"SELECT Id FROM Grupos WITH (UPDLOCK, ROWLOCK) WHERE Id = {grupoId}");

    var pago = await context.Pagos
        .FirstOrDefaultAsync(p => p.Id == pagoId && p.GrupoId == grupoId)
        ?? throw new NoEncontradoException("El pago no existe.");

    if (pago.Anulado)
        throw new ReglaDeNegocioException("El pago ya está anulado.");

    // Solo quien participó del pago o el administrador
    if (usuarioId != pago.DeudorId && usuarioId != pago.AcreedorId
        && usuarioId != grupo.AdministradorId)
        throw new AccesoDenegadoException("No podés anular este pago.");

    pago.Anulado = true;
    pago.FechaAnulacion = DateTime.UtcNow;
    pago.AnuladoPorId = usuarioId;

    await context.SaveChangesAsync();
    await transaccion.CommitAsync();
       }
    }
}
